using Microsoft.VisualBasic;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FM
{
    public partial class AddInvestment : Form
    {
        // ---- UI ----
        private Label lblTitle;

        private Label lblName;
        private TextBox txtName;

        private Label lblAmount;
        private Label lblPound;
        private TextBox txtAmount;

        private Label lblCategory;
        private ComboBox cbCategory;

        private Label lblLength;
        private ComboBox cbLength;

        private Label lblDate;
        private DateTimePicker dtDate;

        private Label lblNotes;
        private TextBox txtNotes;

        private Button btnSave;
        private Button btnViewBills;         // keeps existing handler name, text will say "View Investments"
        private Button btnViewAllPayments;
        private Button btnMainMenu;
        private Button btnCalculateInvestments;

        private Panel bottomPanel;
        private PictureBox logo;

        public AddInvestment()
        {
            // ---- Form styling (match app) ----
            Text = "Add Investment";
            ClientSize = new Size(560, 620);
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;
            Font = new Font("Montserrat", 10, FontStyle.Regular);
            Paint += AddInvestment_Paint;

            // Ensure categories are loaded/bound as soon as the form is ready (keep your original behavior)
            this.Load += AddInvestment_Load;

            SuspendLayout();

            // (Optional) top-center logo
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
                Text = "Add Investment",
                Location = new Point((ClientSize.Width - 190) / 2, 120),
                AutoSize = true,
                Font = new Font("Montserrat", 14F, FontStyle.Bold),
                BackColor = Color.Transparent
            };

            // Name
            lblName = new Label { Text = "Name", Location = new Point(20, 160), AutoSize = true, BackColor = Color.Transparent, TabIndex = 0 };
            txtName = new TextBox { Location = new Point(160, 156), Width = 330, TabIndex = 1 };

            // Amount
            lblAmount = new Label { Text = "Amount", Location = new Point(20, 200), AutoSize = true, BackColor = Color.Transparent, TabIndex = 2 };
            lblPound = new Label { Text = "£", Location = new Point(160, 200), AutoSize = true, BackColor = Color.Transparent };
            txtAmount = new TextBox { Location = new Point(175, 196), Width = 120, TabIndex = 3 };

            // Category
            lblCategory = new Label { Text = "Category", Location = new Point(20, 240), AutoSize = true, BackColor = Color.Transparent, TabIndex = 4 };
            cbCategory = new ComboBox
            {
                Location = new Point(160, 236),
                Width = 240,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 5
            };

            // Length
            lblLength = new Label { Text = "Length", Location = new Point(20, 280), AutoSize = true, BackColor = Color.Transparent, TabIndex = 6 };
            cbLength = new ComboBox
            {
                Location = new Point(160, 276),
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 7
            };
            cbLength.Items.AddRange(new object[] { "One-time", "Monthly", "Quarterly", "Yearly" });

            // Date
            lblDate = new Label { Text = "Date", Location = new Point(20, 320), AutoSize = true, BackColor = Color.Transparent, TabIndex = 8 };
            dtDate = new DateTimePicker
            {
                Location = new Point(160, 316),
                Width = 200,
                Format = DateTimePickerFormat.Short,
                TabIndex = 9
            };

            // Notes
            lblNotes = new Label { Text = "Notes", Location = new Point(20, 360), AutoSize = true, BackColor = Color.Transparent, TabIndex = 10 };
            txtNotes = new TextBox
            {
                Location = new Point(160, 356),
                Width = 330,
                Height = 150,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                TabIndex = 11
            };

            // Bottom buttons (transparent so gradient shows)
            bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = Color.Transparent
            };

            btnSave = MakePrimaryButton("Save", new Point(20, 20), 150, 40, 12, btnSave_Click);
            btnViewBills = MakeSecondaryButton("View Investments", new Point(180, 20), 170, 40, 13, btnViewBills_Click);
            btnMainMenu = MakeSecondaryButton("Main Menu", new Point(360, 20), 150, 40, 14, BtnMainMenu_Click);
            btnViewAllPayments = MakeSecondaryButton("View All Payments", new Point(80, 70), 200, 40, 15, btnViewAllPayments_Click);
            btnCalculateInvestments = MakeSecondaryButton("Calculate Investments", new Point(290, 70), 200, 40, 16, btnCalculateInvestments_Click);

            bottomPanel.Controls.AddRange(new Control[] { btnSave, btnViewBills, btnMainMenu, btnViewAllPayments, btnCalculateInvestments });

            Controls.AddRange(new Control[]
            {
                lblTitle,
                lblName, txtName,
                lblAmount, lblPound, txtAmount,
                lblCategory, cbCategory,
                lblLength, cbLength,
                lblDate, dtDate,
                lblNotes, txtNotes,
                bottomPanel
            });

            ResumeLayout(false);
        }

        // ---- Button style helpers (soft red + black border) ----
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

        // ---- Gradient background (LightCoral -> White) ----
        private void AddInvestment_Paint(object? sender, PaintEventArgs e)
        {
            using var brush = new LinearGradientBrush(ClientRectangle, Color.LightCoral, Color.White, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        // ================== Your original logic (kept) ==================

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
                if (cbCategory.DataSource == null || cbCategory.Items.Count == 0)
                    EnsureSchemaAndSeedCategories();

                if (cbCategory.SelectedIndex < 0 && cbCategory.Items.Count > 0)
                    cbCategory.SelectedIndex = 0;

                var item = cbCategory.SelectedItem as CategoryItem;
                int investment_category_id = item?.Id ?? 0;
                string investment_category = item?.Name ?? string.Empty;

                var rec = new InvestmentRecord
                {
                    Investment_ID = "", // will be set after DB insert (RETURNING)
                    Name = txtName.Text.Trim(),
                    Amount = txtAmount.Text.Trim(),   // parsed to decimal during insert
                    Date = dtDate.Value.Date,
                    Category = investment_category,
                    Length = cbLength.SelectedItem?.ToString() ?? cbLength.Text ?? "One-time",
                    Description = txtNotes.Text?.Trim() ?? ""
                };

                if (string.IsNullOrWhiteSpace(rec.Name))
                    throw new InvalidOperationException("Please enter a Name.");

                if (string.IsNullOrWhiteSpace(rec.Amount))
                    throw new InvalidOperationException("Please enter an Amount.");

                if (investment_category_id <= 0)
                    throw new InvalidOperationException("Selected category is invalid.");

                InsertInvestmentsToDb(rec, investment_category_id, investment_category);

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

        private void btnCalculateInvestments_Click(object? sender, EventArgs e)
        {
            var calculate = new CalculateInvestments();
            calculate.Show();
        }

        private void EnsureSchemaAndSeedCategories()
        {
            using var conn = new NpgsqlConnection(ConnStr);
            conn.Open();

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

            using (var cmd = new NpgsqlCommand(@"
ALTER TABLE investments ADD COLUMN IF NOT EXISTS date        DATE;
ALTER TABLE investments ADD COLUMN IF NOT EXISTS category_id INT;
ALTER TABLE investments ADD COLUMN IF NOT EXISTS category    TEXT;
ALTER TABLE investments ADD COLUMN IF NOT EXISTS length      TEXT;
ALTER TABLE investments ADD COLUMN IF NOT EXISTS notes       TEXT;", conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new NpgsqlCommand(@"
ALTER TABLE investments DROP CONSTRAINT IF EXISTS fk_investments_category;
ALTER TABLE investments
  ADD CONSTRAINT fk_investments_category
  FOREIGN KEY (category_id) REFERENCES investment_categories(id);", conn))
            {
                cmd.ExecuteNonQuery();
            }

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

            var list = new List<CategoryItem>();
            using (var cmd = new NpgsqlCommand(
                "SELECT id, name FROM investment_categories ORDER BY name;", conn))
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                    list.Add(new CategoryItem(rdr.GetInt32(0), rdr.GetString(1)));
            }

            cbCategory.DataSource = null;
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
            if (newIdObj != null && newIdObj != DBNull.Value)
                rec.Investment_ID = Convert.ToString(newIdObj, CultureInfo.InvariantCulture) ?? string.Empty;
        }
    }
}
