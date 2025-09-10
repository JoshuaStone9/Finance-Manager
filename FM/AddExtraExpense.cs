using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace FM
{
    public partial class AddExtraExpense : Form
    {
        private Label lblTitle;

        private Label lblName;
        private TextBox txtName;

        private Label lblAmount;
        private Label lblPound;
        private TextBox txtAmount;

        private Label lblCategory;
        private ComboBox cboCategory;

        private Label lblType;          // One-off | Recurring
        private ComboBox cboType;

        private Label lblFrequency;     // visible only when Recurring
        private ComboBox cboFrequency;  // Weekly | Monthly | Quarterly | Yearly

        private Label lblDate;
        private DateTimePicker dtpDate;

        private Label lblNotes;
        private TextBox txtNotes;

        private Button btnSave;
        private Button btnViewExpenses;
        private Button btnViewAllPayments;
        private Button btnMainMenu;

        private Panel bottomPanel;
        private PictureBox logo;

        private const string ConnStr =
            "Host=localhost;Database=Finance_Manager;Username=postgres;Password=banana001;SslMode=Disable";

        private sealed class CategoryItem
        {
            public int Id { get; }
            public string Name { get; }
            public CategoryItem(int id, string name) { Id = id; Name = name; }
            public override string ToString() => Name;
        }

        public AddExtraExpense()
        {
            // --- Form (matches your AddBill style) ---
            Text = "Add Extra Expense";
            ClientSize = new Size(560, 620);
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;
            Font = new Font("Montserrat", 10, FontStyle.Regular);
            Paint += AddExtraExpense_Paint;

            SuspendLayout();

            // Optional logo
            logo = new PictureBox
            {
                Image = Image.FromFile("images/FM_Logo_Main_Menu.png"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(120, 120),
                Location = new Point((ClientSize.Width - 120) / 2, 0),
                BackColor = Color.Transparent
            };
            Controls.Add(logo);

            // Title
            lblTitle = new Label
            {
                Text = "Add Extra Expense",
                Location = new Point((ClientSize.Width - 220) / 2, 120),
                AutoSize = true,
                Font = new Font("Montserrat", 14F, FontStyle.Bold),
                BackColor = Color.Transparent
            };

            // Name (shifted up to occupy former ID space)
            lblName = new Label { Text = "Name", Location = new Point(20, 160), AutoSize = true, BackColor = Color.Transparent, TabIndex = 0 };
            txtName = new TextBox { Location = new Point(160, 156), Width = 330, TabIndex = 1 };

            // Amount
            lblAmount = new Label { Text = "Amount", Location = new Point(20, 200), AutoSize = true, BackColor = Color.Transparent, TabIndex = 2 };
            lblPound = new Label { Text = "£", Location = new Point(160, 200), AutoSize = true, BackColor = Color.Transparent };
            txtAmount = new TextBox { Location = new Point(175, 196), Width = 120, TabIndex = 3 };

            // Category
            lblCategory = new Label { Text = "Category", Location = new Point(20, 240), AutoSize = true, BackColor = Color.Transparent, TabIndex = 4 };
            cboCategory = new ComboBox
            {
                Location = new Point(160, 236),
                Width = 240,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 5
            };

            // Type (One-off / Recurring)
            lblType = new Label { Text = "Type", Location = new Point(20, 280), AutoSize = true, BackColor = Color.Transparent, TabIndex = 6 };
            cboType = new ComboBox
            {
                Location = new Point(160, 276),
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 7
            };
            cboType.Items.AddRange(new object[] { "One-off", "Recurring" });

            // Frequency (only when Recurring)
            lblFrequency = new Label
            {
                Text = "Frequency",
                Location = new Point(20, 320),
                AutoSize = true,
                BackColor = Color.Transparent,
                Visible = false,
                TabIndex = 8
            };
            cboFrequency = new ComboBox
            {
                Location = new Point(160, 316),
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Visible = false,
                TabIndex = 9
            };
            cboFrequency.Items.AddRange(new object[] { "Weekly", "Monthly", "Quarterly", "Yearly" });

            // Date Incurred
            lblDate = new Label { Text = "Date Incurred", Location = new Point(20, 360), AutoSize = true, BackColor = Color.Transparent, TabIndex = 10 };
            dtpDate = new DateTimePicker
            {
                Location = new Point(160, 356),
                Width = 200,
                Format = DateTimePickerFormat.Short,
                TabIndex = 11
            };

            // Notes
            lblNotes = new Label { Text = "Notes", Location = new Point(20, 400), AutoSize = true, BackColor = Color.Transparent, TabIndex = 12 };
            txtNotes = new TextBox
            {
                Location = new Point(160, 396),
                Width = 330,
                Height = 110,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                TabIndex = 13
            };

            // Bottom button panel
            bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = Color.Transparent
            };

            btnSave = MakePrimaryButton("Save", new Point(20, 20), 150, 40, 14, BtnSave_Click);
            btnViewExpenses = MakeSecondaryButton("View Expenses", new Point(180, 20), 170, 40, 15, BtnViewExpenses_Click);
            btnMainMenu = MakeSecondaryButton("Main Menu", new Point(360, 20), 150, 40, 16, BtnMainMenu_Click);
            btnViewAllPayments = MakeSecondaryButton("View All Payments", new Point(180, 70), 200, 40, 17, BtnViewAllPayments_Click);

            bottomPanel.Controls.AddRange(new Control[] { btnSave, btnViewExpenses, btnMainMenu, btnViewAllPayments });

            Controls.AddRange(new Control[]
            {
                lblTitle,
                lblName, txtName,
                lblAmount, lblPound, txtAmount,
                lblCategory, cboCategory,
                lblType, cboType,
                lblFrequency, cboFrequency,
                lblDate, dtpDate,
                lblNotes, txtNotes,
                bottomPanel
            });

            // Defaults + events
            cboType.SelectedIndex = 0;
            cboType.SelectedIndexChanged += CboType_SelectedIndexChanged;

            // DB init + load categories
            try { EnsureSchemaAndSeedCategories(); }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization failed:\n{ex.Message}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ResumeLayout(false);
        }

        // ---- Style helpers (match AddBill) ----
        private Button MakePrimaryButton(string text, Point location, int width, int height, int tabIndex, EventHandler onClick)
        {
            var b = new Button
            {
                Text = text,
                Location = location,
                Width = width,
                Height = height,
                TabIndex = tabIndex,
                BackColor = Color.FromArgb(255, 120, 120),
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderColor = Color.Black;
            b.FlatAppearance.BorderSize = 2;
            b.Click += onClick;
            return b;
        }

        private Button MakeSecondaryButton(string text, Point location, int width, int height, int tabIndex, EventHandler onClick)
        {
            var b = new Button
            {
                Text = text,
                Location = location,
                Width = width,
                Height = height,
                TabIndex = tabIndex,
                BackColor = Color.FromArgb(255, 150, 150),
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderColor = Color.Black;
            b.FlatAppearance.BorderSize = 2;
            b.Click += onClick;
            return b;
        }

        // ---- Gradient background ----
        private void AddExtraExpense_Paint(object? sender, PaintEventArgs e)
        {
            using var brush = new LinearGradientBrush(ClientRectangle, Color.LightCoral, Color.White, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        // ---- UI logic ----
        private void CboType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            bool recurring = string.Equals(cboType.SelectedItem?.ToString(), "Recurring", StringComparison.OrdinalIgnoreCase);
            lblFrequency.Visible = recurring;
            cboFrequency.Visible = recurring;
            if (recurring && cboFrequency.SelectedIndex < 0)
                cboFrequency.SelectedIndex = 1; // Monthly
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

            int categoryId = (cboCategory.SelectedValue is int v) ? v : 0;
            string categoryName = (cboCategory.SelectedItem as CategoryItem)?.Name ?? string.Empty;

            var rec = new ExtraExpenseRecord
            {
                // Expense_ID removed
                Name = txtName.Text.Trim(),
                Amount = amount,
                Category = categoryName,
                Type = cboType.SelectedItem?.ToString() ?? "One-off",
                Frequency = (cboFrequency.Visible ? (cboFrequency.SelectedItem?.ToString() ?? "Monthly") : "N/A"),
                DateIncurred = dtpDate.Value.Date,
                Notes = txtNotes.Text.Trim()
            };

            try
            {
                EnsureSchemaAndSeedCategories();

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

        private void BtnViewExpenses_Click(object? sender, EventArgs e) => new ExtraExpensesRecord().Show();
        private void BtnMainMenu_Click(object? sender, EventArgs e) => Close();
        private void BtnViewAllPayments_Click(object? sender, EventArgs e) => new AllPayments().Show();

        // ---- Validation & helpers (Expense ID references removed) ----
        private string? ValidateInputs(out decimal amount)
        {
            amount = 0m;

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
            txtName.BackColor = SystemColors.Window;
            txtAmount.BackColor = SystemColors.Window;
            cboCategory.BackColor = SystemColors.Window;
            cboFrequency.BackColor = SystemColors.Window;
            dtpDate.CalendarMonthBackground = SystemColors.Window;
        }

        private void ClearForNext()
        {
            txtName.Clear();
            txtAmount.Clear();

            if (cboCategory.DataSource != null && cboCategory.Items.Count > 0)
                cboCategory.SelectedIndex = 0;

            cboType.SelectedIndex = 0;
            cboFrequency.SelectedIndex = -1;
            lblFrequency.Visible = cboFrequency.Visible = false;
            dtpDate.Value = DateTime.Today;
            txtNotes.Clear();
            txtName.Focus();
        }

        // ---- DB (unchanged) ----
        private void EnsureSchemaAndSeedCategories()
        {
            using var conn = new NpgsqlConnection(ConnStr);
            conn.Open();

            using (var cmd = new NpgsqlCommand(@"
                CREATE TABLE IF NOT EXISTS categories(
                    id SERIAL PRIMARY KEY,
                    name TEXT NOT NULL UNIQUE
                );

                CREATE TABLE IF NOT EXISTS extra_expenses(
                    extra_expense_id SERIAL PRIMARY KEY,
                    name   TEXT NOT NULL,
                    amount NUMERIC(12,2) NOT NULL
                );", conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new NpgsqlCommand(@"
                ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS category_id INT;
                ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS category    TEXT;
                ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS type        TEXT;
                ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS length      TEXT;
                ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS duedate     DATE;
                ALTER TABLE extra_expenses ADD COLUMN IF NOT EXISTS description TEXT;
            ", conn))
            {
                cmd.ExecuteNonQuery();
            }

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

            using (var cmd = new NpgsqlCommand(@"
                UPDATE extra_expenses e
                SET category_id = c.id
                FROM categories c
                WHERE e.category_id IS NULL
                  AND e.category IS NOT NULL
                  AND lower(e.category) = lower(c.name);

                UPDATE extra_expenses e
                SET category = c.name
                FROM categories c
                WHERE (e.category IS NULL OR e.category = '')
                  AND e.category_id = c.id;", conn))
            {
                cmd.ExecuteNonQuery();
            }

            var list = new List<CategoryItem>();
            using (var cmd = new NpgsqlCommand("SELECT id, name FROM categories ORDER BY name;", conn))
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                    list.Add(new CategoryItem(rdr.GetInt32(0), rdr.GetString(1)));
            }

            cboCategory.DataSource = null;
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
