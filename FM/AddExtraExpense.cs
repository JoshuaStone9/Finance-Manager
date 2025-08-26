using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace FM
{
    public partial class AddExtraExpense : Form
    {
        private Label lblTitle;

        private Label lblExpense_ID;
        private TextBox txtExpense_ID;

        private Label lblName;
        private TextBox txtName;

        private Label lblAmount;
        private Label lblPound;
        private TextBox txtAmount;

        private Label lblCategory;
        private ComboBox cboCategory;

        private Label lblType;
        private ComboBox cboType;           // One-off | Recurring

        private Label lblFrequency;         // visible only when Recurring
        private ComboBox cboFrequency;      // Weekly | Monthly | Quarterly | Yearly

        private Label lblDate;
        private DateTimePicker dtpDate;

        private Label lblNotes;
        private TextBox txtNotes;

        private Button btnSave;
        private Button btnViewExpenses;
        private Button btnViewAllPayments;
        private Button btnMainMenu;

        private Panel bottomPanel;

        private const string ConnStr =
            "Host=localhost;Database=Finance_Manager;Username=postgres;Password=banana001;SslMode=Disable";

        // Helper class for binding
        private sealed class CategoryItem
        {
            public int Id { get; }
            public string Name { get; }
            public CategoryItem(int id, string name) { Id = id; Name = name; }
            public override string ToString() => Name;
        }

        public AddExtraExpense()
        {
            Text = "Add Extra Expense";
            ClientSize = new Size(560, 540);
            StartPosition = FormStartPosition.CenterScreen;

            bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 100 };

            lblTitle = new Label
            {
                Text = "Add Extra Expense",
                Location = new Point(190, 12),
                AutoSize = true,
                Font = new Font("Cambria", 14F, FontStyle.Regular)
            };

            // Expense ID
            lblExpense_ID = new Label { Text = "Expense ID", Location = new Point(20, 60), AutoSize = true, TabIndex = 0 };
            txtExpense_ID = new TextBox { Location = new Point(160, 56), Width = 180, TabIndex = 1 };

            // Name
            lblName = new Label { Text = "Name", Location = new Point(20, 100), AutoSize = true, TabIndex = 2 };
            txtName = new TextBox { Location = new Point(160, 96), Width = 330, TabIndex = 3 };

            // Amount
            lblAmount = new Label { Text = "Amount", Location = new Point(20, 140), AutoSize = true, TabIndex = 4 };
            lblPound = new Label { Text = "£", Location = new Point(160, 140), AutoSize = true };
            txtAmount = new TextBox { Location = new Point(175, 136), Width = 120, TabIndex = 5 };

            // Category
            lblCategory = new Label { Text = "Category", Location = new Point(20, 180), AutoSize = true, TabIndex = 6 };
            cboCategory = new ComboBox
            {
                Location = new Point(160, 176),
                Width = 240,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 7
            };
            // Items are loaded from DB in EnsureSchemaAndSeedCategories()

            // Type (One-off / Recurring)
            lblType = new Label { Text = "Type", Location = new Point(20, 220), AutoSize = true, TabIndex = 8 };
            cboType = new ComboBox
            {
                Location = new Point(160, 216),
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 9
            };
            cboType.Items.AddRange(new object[] { "One-off", "Recurring" });

            // Frequency
            lblFrequency = new Label
            {
                Text = "Frequency",
                Location = new Point(20, 260),
                AutoSize = true,
                Visible = false,
                TabIndex = 10
            };
            cboFrequency = new ComboBox
            {
                Location = new Point(160, 256),
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Visible = false,
                TabIndex = 11
            };
            cboFrequency.Items.AddRange(new object[] { "Weekly", "Monthly", "Quarterly", "Yearly" });

            // Date Incurred
            lblDate = new Label { Text = "Date Incurred", Location = new Point(20, 300), AutoSize = true, TabIndex = 12 };
            dtpDate = new DateTimePicker
            {
                Location = new Point(160, 296),
                Width = 200,
                Format = DateTimePickerFormat.Short,
                TabIndex = 13
            };

            // Notes
            lblNotes = new Label { Text = "Notes", Location = new Point(20, 340), AutoSize = true, TabIndex = 14 };
            txtNotes = new TextBox
            {
                Location = new Point(160, 336),
                Width = 330,
                Height = 110,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                TabIndex = 15
            };

            // Buttons
            btnSave = new Button { Text = "Save", Location = new Point(160, 15), Width = 120, Height = 36, TabIndex = 16 };
            btnSave.Click += BtnSave_Click;

            btnViewExpenses = new Button { Text = "View Expenses", Location = new Point(300, 15), Width = 200, Height = 36, TabIndex = 17 };
            btnViewExpenses.Click += BtnViewExpenses_Click;

            btnViewAllPayments = new Button { Text = "View All Payments", Location = new Point(300, 60), Width = 200, Height = 35, TabIndex = 18 };
            btnViewAllPayments.Click += BtnViewAllPayments_Click;

            btnMainMenu = new Button { Text = "Main Menu", Location = new Point(160, 60), Width = 120, Height = 35, TabIndex = 19 };
            btnMainMenu.Click += BtnMainMenu_Click;

            bottomPanel.Controls.AddRange(new Control[] { btnSave, btnViewExpenses, btnMainMenu, btnViewAllPayments });

            Controls.AddRange(new Control[]
            {
                lblTitle,
                lblExpense_ID, txtExpense_ID,
                lblName, txtName,
                lblAmount, lblPound, txtAmount,
                lblCategory, cboCategory,
                lblType, cboType,
                lblFrequency, cboFrequency,
                lblDate, dtpDate,
                lblNotes, txtNotes,
                bottomPanel,
            });

            // Defaults + events
            cboType.SelectedIndex = 0;
            cboType.SelectedIndexChanged += CboType_SelectedIndexChanged;

            // Ensure schema, seed, and load categories
            try
            {
                EnsureSchemaAndSeedCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization failed:\n{ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CboType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            bool recurring = string.Equals(cboType.SelectedItem?.ToString(), "Recurring", StringComparison.OrdinalIgnoreCase);
            lblFrequency.Visible = recurring;
            cboFrequency.Visible = recurring;
            if (recurring && cboFrequency.SelectedIndex < 0)
                cboFrequency.SelectedIndex = 1; // default Monthly
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            ResetFieldBackColors();

            string? error = ValidateInputs(out decimal amount);
            if (error != null)
            {
                MessageBox.Show(error, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get both the id and name from the bound combo
            int categoryId = (cboCategory.SelectedValue is int v) ? v : 0;
            string categoryName = (cboCategory.SelectedItem as CategoryItem)?.Name ?? string.Empty;

            var rec = new ExtraExpenseRecord
            {
                Expense_ID = txtExpense_ID.Text.Trim(),
                Name = txtName.Text.Trim(),
                Amount = amount,
                Category = categoryName,  // keep name in memory/store if you use it elsewhere
                Type = cboType.SelectedItem?.ToString() ?? "One-off",
                Frequency = (cboFrequency.Visible ? (cboFrequency.SelectedItem?.ToString() ?? "Monthly") : "N/A"),
                DateIncurred = dtpDate.Value.Date,
                Notes = txtNotes.Text.Trim()
            };

            try
            {
                EnsureSchemaAndSeedCategories(); // idempotent

                if (categoryId <= 0)
                    throw new InvalidOperationException("Selected category is invalid.");

                InsertExtraExpenseToDb(rec, categoryId, categoryName);

                ExtraExpenseStore.Expenses.Add(rec);

                MessageBox.Show("Expense saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForNext();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not save expense:\n{ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnViewExpenses_Click(object? sender, EventArgs e)
        {
            var expenses = new ExtraExpensesRecord();
            expenses.Show();
        }

        private void BtnMainMenu_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnViewAllPayments_Click(object? sender, EventArgs e)
        {
            var form = new AllPayments();
            form.Show();
        }

        // ---- helpers ----

        private string? ValidateInputs(out decimal amount)
        {
            amount = 0m;

            if (string.IsNullOrWhiteSpace(txtExpense_ID.Text))
            {
                txtExpense_ID.BackColor = Color.MistyRose;
                return "Please enter an Expense ID.";
            }

            if (ExtraExpenseStore.Expenses.Any(x =>
                string.Equals(x.Expense_ID, txtExpense_ID.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                txtExpense_ID.BackColor = Color.MistyRose;
                return "That Expense ID already exists. Use a unique ID.";
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                txtName.BackColor = Color.MistyRose;
                return "Please enter a Name.";
            }

            if (!decimal.TryParse(txtAmount.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out amount) || amount <= 0)
            {
                txtAmount.BackColor = Color.MistyRose;
                return "Please enter a valid Amount greater than 0.";
            }

            if (cboCategory.SelectedValue is not int)
            {
                cboCategory.BackColor = Color.MistyRose;
                return "Please select a Category.";
            }

            if (string.Equals(cboType.SelectedItem?.ToString(), "Recurring", StringComparison.OrdinalIgnoreCase)
                && (cboFrequency.SelectedIndex < 0))
            {
                cboFrequency.BackColor = Color.MistyRose;
                return "Please choose a Frequency for Recurring expenses.";
            }

            if (dtpDate.Value.Date > DateTime.Today)
            {
                dtpDate.CalendarMonthBackground = Color.MistyRose;
                return "Date Incurred cannot be in the future.";
            }

            return null;
        }

        private void ResetFieldBackColors()
        {
            txtExpense_ID.BackColor = SystemColors.Window;
            txtName.BackColor = SystemColors.Window;
            txtAmount.BackColor = SystemColors.Window;
            cboCategory.BackColor = SystemColors.Window;
            cboFrequency.BackColor = SystemColors.Window;
            dtpDate.CalendarMonthBackground = SystemColors.Window;
        }

        private void ClearForNext()
        {
            txtExpense_ID.Clear();
            txtName.Clear();
            txtAmount.Clear();

            // Reset selection safely for a bound combo
            if (cboCategory.DataSource != null && cboCategory.Items.Count > 0)
                cboCategory.SelectedIndex = 0;

            cboType.SelectedIndex = 0;
            cboFrequency.SelectedIndex = -1;
            lblFrequency.Visible = cboFrequency.Visible = false;
            dtpDate.Value = DateTime.Today;
            txtNotes.Clear();
            txtExpense_ID.Focus();
        }

        private void EnsureSchemaAndSeedCategories()
        {
            using var conn = new NpgsqlConnection(ConnStr);
            conn.Open();

            // 1) Create base tables if missing
            using (var cmd = new NpgsqlCommand(@"
                CREATE TABLE IF NOT EXISTS categories(
                    id SERIAL PRIMARY KEY,
                    name TEXT NOT NULL UNIQUE
                );

                CREATE TABLE IF NOT EXISTS extra_expenses(
                    extra_expense_id SERIAL PRIMARY KEY,
                    name TEXT NOT NULL,
                    amount NUMERIC(12,2) NOT NULL
                );", conn))
            {
                cmd.ExecuteNonQuery();
            }

            // 2) Add/ensure columns (idempotent)
            using (var cmd = new Npgsql.NpgsqlCommand(@"
                ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS category_id INT;
                ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS category    TEXT; -- keep legacy text too
                ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS type        TEXT;
                ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS length      TEXT;
                ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS duedate     DATE;
                ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS description TEXT;
            ", conn))
            {
                cmd.ExecuteNonQuery();
            }

            // 3) Foreign key (best-effort)
            using (var cmd = new NpgsqlCommand(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.table_constraints
                        WHERE table_name = 'extra_expenses'
                          AND constraint_type = 'FOREIGN KEY'
                          AND constraint_name = 'extra_expenses_category_id_fkey'
                    ) THEN
                        ALTER TABLE extra_expenses
                        ADD CONSTRAINT extra_expenses_category_id_fkey
                        FOREIGN KEY (category_id) REFERENCES categories(id);
                    END IF;
                EXCEPTION WHEN others THEN
                    NULL;
                END $$;", conn))
            {
                cmd.ExecuteNonQuery();
            }

            // 4) Seed categories
            string[] categories = {
                "Groceries","Transport","Entertainment","Dining Out",
                "Utilities","Health","Education","Gifts","Home",
                "Personal","Travel","Other"
            };

            using (var tx = conn.BeginTransaction())
            {
                foreach (var c in categories)
                {
                    using var upsert = new NpgsqlCommand(
                        "INSERT INTO categories(name) VALUES (@name) ON CONFLICT (name) DO NOTHING;", conn, tx);
                    upsert.Parameters.AddWithValue("@name", NpgsqlDbType.Text, c);
                    upsert.ExecuteNonQuery();
                }
                tx.Commit();
            }

            // 5) Backfill either way (legacy rows)
            using (var cmd = new NpgsqlCommand(@"
                -- Fill category_id from category name (legacy)
                UPDATE extra_expenses e
                SET category_id = c.id
                FROM categories c
                WHERE e.category_id IS NULL
                  AND e.category IS NOT NULL
                  AND lower(e.category) = lower(c.name);

                -- Fill category text from category_id if missing
                UPDATE extra_expenses e
                SET category = c.name
                FROM categories c
                WHERE (e.category IS NULL OR e.category = '')
                  AND e.category_id = c.id;", conn))
            {
                cmd.ExecuteNonQuery();
            }

            // 6) Load categories into the ComboBox using proper data binding
            var list = new List<CategoryItem>();
            using (var cmd = new NpgsqlCommand("SELECT id, name FROM categories ORDER BY name;", conn))
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                    list.Add(new CategoryItem(rdr.GetInt32(0), rdr.GetString(1)));
            }

            cboCategory.DataSource = null; // reset any previous binding/items
            cboCategory.DisplayMember = nameof(CategoryItem.Name);
            cboCategory.ValueMember = nameof(CategoryItem.Id);
            cboCategory.DataSource = list;

            if (cboCategory.Items.Count > 0 && cboCategory.SelectedIndex < 0)
                cboCategory.SelectedIndex = 0;
        }

        private void InsertExtraExpenseToDb(ExtraExpenseRecord rec, int categoryId, string categoryName)
        {
            using var conn = new NpgsqlConnection(ConnStr);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
                INSERT INTO extra_expenses
                    (name, amount, category_id, category, type, length, duedate, description)
                VALUES
                    (@n,   @a,     @cat_id,    @cat,     @type, @freq,  @d,     @desc);", conn);

            cmd.Parameters.AddWithValue("@n", NpgsqlDbType.Text, rec.Name);
            cmd.Parameters.AddWithValue("@a", NpgsqlDbType.Numeric, rec.Amount);
            cmd.Parameters.AddWithValue("@cat_id", NpgsqlDbType.Integer, categoryId);
            cmd.Parameters.AddWithValue("@cat", NpgsqlDbType.Text, categoryName);
            cmd.Parameters.AddWithValue("@type", NpgsqlDbType.Text, rec.Type);

            if (string.Equals(rec.Type, "Recurring", StringComparison.OrdinalIgnoreCase))
                cmd.Parameters.AddWithValue("@freq", NpgsqlDbType.Text, rec.Frequency);
            else
                cmd.Parameters.AddWithValue("@freq", DBNull.Value);

            cmd.Parameters.AddWithValue("@d", NpgsqlDbType.Date, rec.DateIncurred);
            if (string.IsNullOrWhiteSpace(rec.Notes))
                cmd.Parameters.AddWithValue("@desc", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@desc", NpgsqlDbType.Text, rec.Notes);

            cmd.ExecuteNonQuery();
        }
    }
}
