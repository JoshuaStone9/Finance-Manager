using Microsoft.VisualBasic;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace FM
{
    public partial class AddInvestment : System.Windows.Forms.Form
    {
        public AddInvestment()
        {
            InitializeComponent();

            // Ensure categories are loaded/bound as soon as the form is ready
            this.Load += AddInvestment_Load;
        }

        private void AddInvestment_Load(object? sender, EventArgs e)
        {
            try
            {
                EnsureSchemaAndSeedCategories(); // creates tables, seeds, binds cbCategory
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load categories: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            try
            {
                // Make sure combo is bound (in case Load didn't complete)
                if (cbCategory.DataSource == null || cbCategory.Items.Count == 0)
                    EnsureSchemaAndSeedCategories();

                // If nothing selected but items exist, select the first
                if (cbCategory.SelectedIndex < 0 && cbCategory.Items.Count > 0)
                    cbCategory.SelectedIndex = 0;

                // Read the actual bound item (Id + Name)
                var item = cbCategory.SelectedItem as CategoryItem;
                int investment_category_id = item?.Id ?? 0;
                string investment_category = item?.Name ?? string.Empty;

                var rec = new InvestmentRecord
                {
                    Investment_ID = txtInvestment_ID.Text.Trim(), // will be overwritten by DB id after insert
                    Name = txtName.Text.Trim(),
                    Amount = txtAmount.Text.Trim(),         // parsed to decimal during insert
                    Date = dtDate.Value.Date,
                    Category = investment_category,           // keep friendly name too
                    Length = cbLength.SelectedItem?.ToString() ?? cbLength.Text ?? "One-time",
                    Description = txtNotes.Text?.Trim() ?? ""
                };

                if (investment_category_id <= 0)
                    throw new InvalidOperationException("Selected category is invalid.");

                InsertInvestmentsToDb(rec, investment_category_id, investment_category);

                // Optional in-memory store (your existing pattern)
                InvestmentStore.Investments.Add(rec);

                MessageBox.Show("Investment saved successfully.",
                                "Success",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving investment: {ex.Message}",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void btnViewBills_Click(object? sender, EventArgs e)
        {
            var investements = new InvestmentsRecord();
            investements.Show();
        }

        private void BtnMainMenu_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void btnViewAllPayments_Click(object? sender, EventArgs e)
        {
            var allPayments = new AllPayments();
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

CREATE TABLE IF NOT EXISTS investments(
    investments_id SERIAL PRIMARY KEY,
    name           TEXT NOT NULL,
    amount         NUMERIC(12,2) NOT NULL
);", conn))
            {
                cmd.ExecuteNonQuery();
            }

            // 2) Add/ensure additional columns (idempotent; correct types)
            using (var cmd = new NpgsqlCommand(@"
ALTER TABLE investments ADD COLUMN IF NOT EXISTS date        DATE;
ALTER TABLE investments ADD COLUMN IF NOT EXISTS category_id INT;
ALTER TABLE investments ADD COLUMN IF NOT EXISTS category    TEXT;
ALTER TABLE investments ADD COLUMN IF NOT EXISTS length      TEXT;
ALTER TABLE investments ADD COLUMN IF NOT EXISTS notes       TEXT;", conn))
            {
                cmd.ExecuteNonQuery();
            }

            // 3) Ensure FK -> investment_categories(id)
            using (var cmd = new NpgsqlCommand(@"
ALTER TABLE investments DROP CONSTRAINT IF EXISTS fk_investments_category;
ALTER TABLE investments
  ADD CONSTRAINT fk_investments_category
  FOREIGN KEY (category_id) REFERENCES investment_categories(id);", conn))
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

            // 5) Backfill legacy rows (keep name/id in sync)
            using (var cmd = new NpgsqlCommand(@"
UPDATE investments e
SET category_id = ic.id
FROM investment_categories ic
WHERE e.category_id IS NULL
  AND e.category IS NOT NULL
  AND lower(e.category) = lower(ic.name);

UPDATE investments e
SET category = ic.name
FROM investment_categories ic
WHERE (e.category IS NULL OR e.category = '')
  AND e.category_id = ic.id;", conn))
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

            cbCategory.DataSource = null; // reset binding
            cbCategory.DisplayMember = nameof(CategoryItem.Name);
            cbCategory.ValueMember = nameof(CategoryItem.Id);
            cbCategory.DataSource = list;

            if (cbCategory.Items.Count > 0 && cbCategory.SelectedIndex < 0)
                cbCategory.SelectedIndex = 0;
        }

        private void InsertInvestmentsToDb(InvestmentRecord rec, int categoryId, string categoryName)
        {
            using var conn = new NpgsqlConnection(ConnStr);
            conn.Open();

            // Parse Amount -> decimal for NUMERIC(12,2)
            if (!decimal.TryParse(rec.Amount, NumberStyles.Number, CultureInfo.CurrentCulture, out var amount) &&
                !decimal.TryParse(rec.Amount, NumberStyles.Number, CultureInfo.InvariantCulture, out amount))
            {
                throw new FormatException("Amount must be a number, e.g. 1234.56");
            }

            using var cmd = new NpgsqlCommand(@"
INSERT INTO investments
    (name, amount, date, category_id, category, length, notes)
VALUES
    (@n,   @a,     @d,   @cat_id,     @cat,    @l,     @notes)
RETURNING investments_id;", conn);

            cmd.Parameters.Add("@n", NpgsqlDbType.Text).Value = rec.Name?.Trim();
            cmd.Parameters.Add("@a", NpgsqlDbType.Numeric).Value = amount;
            cmd.Parameters.Add("@d", NpgsqlDbType.Date).Value = rec.Date.Date;
            cmd.Parameters.Add("@cat_id", NpgsqlDbType.Integer).Value = categoryId;
            cmd.Parameters.Add("@cat", NpgsqlDbType.Text).Value = categoryName;
            cmd.Parameters.Add("@l", NpgsqlDbType.Text).Value = rec.Length ?? "One-time";
            cmd.Parameters.Add("@notes", NpgsqlDbType.Text).Value = (object?)rec.Description ?? DBNull.Value;

            var newIdObj = cmd.ExecuteScalar();
            int newId = Convert.ToInt32(newIdObj);
            if (newIdObj != null && newIdObj != DBNull.Value)
                rec.Investment_ID = Convert.ToString(newIdObj, CultureInfo.InvariantCulture) ?? string.Empty;
        }
    }
}
