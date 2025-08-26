using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;
using System.Windows.Forms;

namespace FM
{
    public partial class AddInvestment : System.Windows.Forms.Form
    {
        public AddInvestment()
        {
            InitializeComponent();
        }
        private const string ConnStr =
            "Host=localhost;Database=Finance_Manager;Username=postgres;Password=banana001;SslMode=Disable";

        private sealed class CategoryItem
        {
            public int Id { get; }
            public string Name { get; }
            public CategoryItem(int id, string name) { Id = id; Name = name; }
            public override string ToString() => Name;
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            var rec = new InvestmentRecord
            {
                Name = txtName.Text,
                Amount = txtAmount.Text,
                Date = dtDate.Value,
                Category = cbCategory.SelectedItem?.ToString() ?? "",
                Length = cbLength.SelectedItem?.ToString() ?? "",
                Description = txtNotes.Text // Fixed: use Description property instead of Notes
            };
            InvestmentStore.Investments.Add(rec);

            int categoryId = (cbCategory.SelectedValue is int v) ? v : 0;
            string categoryName = (cbCategory.SelectedItem as CategoryItem)?.Name ?? string.Empty;
        }

        private void btnViewBills_Click(object? sender, EventArgs e)
        {
            InvestmentsRecord investements = new InvestmentsRecord();
            investements.Show();
        }

        private void BtnMainMenu_Click(object? sender, EventArgs e)
        {
            this.Close();
        }



        private void btnViewAllPayments_Click(object? sender, EventArgs e)
        {
            AllPayments allPayments = new AllPayments();
            allPayments.Show();
        }

        private void EnsureSchemaAndSeedCategories()
        {
            using var conn = new NpgsqlConnection(ConnStr);
            conn.Open();

            // 1) Create base tables if missing
            using (var cmd = new NpgsqlCommand(@"
        CREATE TABLE IF NOT EXISTS investment_categories(
            id   SERIAL PRIMARY KEY,
            name TEXT NOT NULL UNIQUE
        );

        CREATE TABLE IF NOT EXISTS extra_expenses(
            extra_expense_id SERIAL PRIMARY KEY,
            name   TEXT NOT NULL,
            amount NUMERIC(12,2) NOT NULL
        );
    ", conn))
            {
                cmd.ExecuteNonQuery();
            }

            // 2) Add/ensure columns (idempotent)
            using (var cmd = new NpgsqlCommand(@"
        ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS category_id INT;
        ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS category     TEXT;   -- keep legacy text too
        ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS type         TEXT;
        ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS length       TEXT;
        ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS duedate      DATE;
        ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS description  TEXT;
    ", conn))
            {
                cmd.ExecuteNonQuery();
            }

            // 3) Ensure FK -> investment_categories (drop old one if it exists)
            using (var cmd = new NpgsqlCommand(@"
        DO $$
        BEGIN
            BEGIN
                ALTER TABLE extra_expenses DROP CONSTRAINT IF EXISTS extra_expenses_category_id_fkey;
            EXCEPTION WHEN others THEN NULL;
            END;

            BEGIN
                ALTER TABLE extra_expenses
                ADD CONSTRAINT extra_expenses_category_id_fkey
                FOREIGN KEY (category_id) REFERENCES investment_categories(id);
            EXCEPTION WHEN others THEN NULL;
            END;
        END $$;
    ", conn))
            {
                cmd.ExecuteNonQuery();
            }

            // 4) Seed investment categories
            string[] inv = { "Stocks", "ETF", "Crypto", "Real Estate" };
            using (var tx = conn.BeginTransaction())
            {
                foreach (var c in inv)
                {
                    using var upsert = new NpgsqlCommand(
                        "INSERT INTO investment_categories(name) VALUES (@name) ON CONFLICT (name) DO NOTHING;",
                        conn, tx);
                    upsert.Parameters.AddWithValue("@name", NpgsqlDbType.Text, c);
                    upsert.ExecuteNonQuery();
                }
                tx.Commit();
            }

            // 5) Backfill either way (legacy rows)
            using (var cmd = new NpgsqlCommand(@"
        -- Fill category_id from legacy category text (if any)
        UPDATE extra_expenses e
        SET category_id = ic.id
        FROM investment_categories ic
        WHERE e.category_id IS NULL
          AND e.category IS NOT NULL
          AND lower(e.category) = lower(ic.name);

        -- Fill category text from category_id if missing
        UPDATE extra_expenses e
        SET category = ic.name
        FROM investment_categories ic
        WHERE (e.category IS NULL OR e.category = '')
          AND e.category_id = ic.id;
    ", conn))
            {
                cmd.ExecuteNonQuery();
            }

            // 6) Load categories into the ComboBox using proper data binding
            var list = new List<CategoryItem>();
            using (var cmd = new NpgsqlCommand(
                "SELECT id, name FROM investment_categories ORDER BY name;", conn))
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                    list.Add(new CategoryItem(rdr.GetInt32(0), rdr.GetString(1)));
            }

            cbCategory.DataSource = null; // reset
            cbCategory.DisplayMember = nameof(CategoryItem.Name);
            cbCategory.ValueMember = nameof(CategoryItem.Id);
            cbCategory.DataSource = list;
            if (cbCategory.Items.Count > 0 && cbCategory.SelectedIndex < 0)
                cbCategory.SelectedIndex = 0;
        }

        private void CreateAndLoadBillsTable()
        {
            string cs = "Host=localhost;Database=Finance_Manager;Username=postgres;Password=banana001;SslMode=Disable";

            using var conn = new NpgsqlConnection(cs);
            conn.Open();

            // Create tables if not exist
            using (var cmd = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS bills(
                billid SERIAL PRIMARY KEY,
                name TEXT NOT NULL,
                amount NUMERIC(12,2) NOT NULL,
                type TEXT,
                length TEXT,
                duedate DATE,
                description TEXT
                );", conn))
            {
                cmd.ExecuteNonQuery();
            }

            // Insert
            using (var cmd = new NpgsqlCommand(
                "INSERT INTO investments(name,amount,date,category,length,duedate,description) VALUES(@n,@a,@d,@cat,@l,@d,@desc)", conn))
            {
                cmd.Parameters.AddWithValue("@n", txtName.Text);
                cmd.Parameters.AddWithValue("@a", decimal.Parse(txtAmount.Text));
                cmd.Parameters.AddWithValue("@d", dtDate.Value);
                cmd.Parameters.AddWithValue("@cat",
                    string.Equals((cbCategory.SelectedItem as CategoryItem)?.Name, "Other", StringComparison.OrdinalIgnoreCase)
                    ? txtOtherInvestment.Text?.Trim() ?? "Other"
                    : (cbCategory.SelectedItem as CategoryItem)?.Name ?? "Not Applicable");
                cmd.Parameters.AddWithValue("@l", cbLength.Visible
                    ? cbLength.SelectedItem?.ToString() ?? "Not Sure"
                    : "Not Applicable");
                cmd.Parameters.AddWithValue("@d", dtDate.Value.Date);
                cmd.Parameters.AddWithValue("@desc", txtNotes.Text);
                cmd.ExecuteNonQuery();
            }

            // Load into DataTable (bind to DataGridView)
            var dt = new DataTable();
            using (var cmd = new NpgsqlCommand("SELECT billid,name,amount,duedate FROM bills ORDER BY duedate DESC", conn))
            using (var rdr = cmd.ExecuteReader()) dt.Load(rdr);

            // e.g. gridBills.DataSource = dt;
        }
    }
}
