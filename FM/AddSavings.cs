using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace FM
{
    public partial class AddSavings : Form
    {
        private Label lblTitle;

        private Label lblName;
        private TextBox txtName;

        private Label lblAmount;
        private Label lblPound;
        private TextBox txtAmount;

        private Label lblLength;
        private ComboBox cboLength;

        private Label lblDate;
        private DateTimePicker dtpDate;

        private Label lblNotes;
        private TextBox txtNotes;

        private Button btnSave;
        private Button btnViewSavings;
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

        public AddSavings()
        {
            // ---- Form styling to match the app ----
            Text = "Add Savings";
            ClientSize = new Size(560, 620);
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;
            Font = new Font("Montserrat", 10, FontStyle.Regular);
            Paint += AddSavings_Paint;

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
                Text = "Add Savings",
                Location = new Point((ClientSize.Width - 160) / 2, 120),
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

            // Length
            lblLength = new Label { Text = "Length", Location = new Point(20, 240), AutoSize = true, BackColor = Color.Transparent, TabIndex = 4 };
            cboLength = new ComboBox
            {
                Location = new Point(160, 236),
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 5
            };
            cboLength.Items.AddRange(new object[] { "Daily", "Weekly", "Monthly", "Quarterly", "Yearly" });

            // Date
            lblDate = new Label { Text = "Date", Location = new Point(20, 280), AutoSize = true, BackColor = Color.Transparent, TabIndex = 6 };
            dtpDate = new DateTimePicker
            {
                Location = new Point(160, 276),
                Width = 200,
                Format = DateTimePickerFormat.Short,
                TabIndex = 7
            };

            // Notes
            lblNotes = new Label { Text = "Notes", Location = new Point(20, 320), AutoSize = true, BackColor = Color.Transparent, TabIndex = 8 };
            txtNotes = new TextBox
            {
                Location = new Point(160, 316),
                Width = 330,
                Height = 150,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                TabIndex = 9
            };

            // Bottom button panel (transparent to show gradient)
            bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = Color.Transparent
            };

            btnSave = MakePrimaryButton("Save", new Point(20, 20), 150, 40, 10, BtnSave_Click);
            btnViewSavings = MakeSecondaryButton("View Savings", new Point(180, 20), 170, 40, 11, BtnViewSavings_Click);
            btnMainMenu = MakeSecondaryButton("Main Menu", new Point(360, 20), 150, 40, 12, BtnMainMenu_Click);
            btnViewAllPayments = MakeSecondaryButton("View All Payments", new Point(180, 70), 200, 40, 13, BtnViewAllPayments_Click);

            bottomPanel.Controls.AddRange(new Control[] { btnSave, btnViewSavings, btnMainMenu, btnViewAllPayments });

            Controls.AddRange(new Control[]
            {
                lblTitle,
                lblName, txtName,
                lblAmount, lblPound, txtAmount,
                lblLength, cboLength,
                lblDate, dtpDate,
                lblNotes, txtNotes,
                bottomPanel
            });

            // Optional: ensure schema (safe, but CreateInsertAndLoadSavings also ensures)
            try { EnsureSchemaAndSeedCategories(); } catch { /* no-op */ }

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
        private void AddSavings_Paint(object? sender, PaintEventArgs e)
        {
            using var brush = new LinearGradientBrush(ClientRectangle, Color.LightCoral, Color.White, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        // ---- Events ----
        private void BtnSave_Click(object? sender, EventArgs e)
        {
            ResetFieldBackColors();

            string? error = ValidateInputs(out decimal amount);
            if (error != null)
            {
                MessageBox.Show(error, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var rec = new SavingsRecord
            {
                Name = txtName.Text.Trim(),
                Amount = amount,
                Length = cboLength.SelectedItem?.ToString() ?? "N/A",
                Date = dtpDate.Value.Date,
                Notes = txtNotes.Text.Trim()
            };

            try
            {
                // Create table (if missing), insert, and load latest data back
                var dt = CreateInsertAndLoadSavings(rec);

                // Keep your in-memory store if you use it elsewhere
                SavingsStore.Savings.Add(rec);

                MessageBox.Show("Savings saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // If you have a grid to show savings elsewhere, you can bind 'dt' there.
                ClearFormForNext();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not save savings:\n{ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnViewSavings_Click(object? sender, EventArgs e)
        {
            // TODO: open your Savings list form when ready
            // new SavingsListForm().Show();
        }

        private void BtnMainMenu_Click(object? sender, EventArgs e) => Close();

        private void BtnViewAllPayments_Click(object? sender, EventArgs e) => new AllPayments().Show();

        // ---- Validation & helpers ----
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

            if (string.IsNullOrWhiteSpace(cboLength.Text))
            {
                cboLength.BackColor = Color.MistyRose;
                return "Please input a length of time.";
            }

            if (dtpDate.Value.Date > DateTime.Today)
            {
                dtpDate.CalendarMonthBackground = Color.MistyRose;
                return "Date cannot be in the future.";
            }

            return null;
        }

        private void ResetFieldBackColors()
        {
            txtName.BackColor = SystemColors.Window;
            txtAmount.BackColor = SystemColors.Window;
            cboLength.BackColor = SystemColors.Window;
            dtpDate.CalendarMonthBackground = SystemColors.Window;
        }

        private void ClearFormForNext()
        {
            txtName.Clear();
            txtAmount.Clear();
            cboLength.SelectedIndex = -1;
            dtpDate.Value = DateTime.Today;
            txtNotes.Clear();
            txtName.Focus();
        }

        // ---- DB schema helper (optional; CreateInsertAndLoadSavings also ensures) ----
        private void EnsureSchemaAndSeedCategories()
        {
            using var conn = new NpgsqlConnection(ConnStr);
            conn.Open();

            using (var cmd = new NpgsqlCommand(@"
                CREATE TABLE IF NOT EXISTS savings(
                    savings_id SERIAL PRIMARY KEY,
                    name   TEXT NOT NULL,
                    amount NUMERIC(12,2) NOT NULL
                );", conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new NpgsqlCommand(@"
                ALTER TABLE savings ADD COLUMN IF NOT EXISTS length TEXT;
                ALTER TABLE savings ADD COLUMN IF NOT EXISTS ""date"" DATE;
                ALTER TABLE savings ADD COLUMN IF NOT EXISTS notes  TEXT;
            ", conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        // ---- Refactored create/insert/load (quotes ""date"" for safety) ----
        private DataTable CreateInsertAndLoadSavings(SavingsRecord rec)
        {
            using var conn = new NpgsqlConnection(ConnStr);
            conn.Open();

            // Ensure table exists
            using (var cmd = new NpgsqlCommand(@"
                CREATE TABLE IF NOT EXISTS savings(
                    savings_id SERIAL PRIMARY KEY,
                    name   TEXT NOT NULL,
                    amount NUMERIC(12,2) NOT NULL,
                    length TEXT,
                    ""date"" DATE,
                    notes  TEXT
                );", conn))
            {
                cmd.ExecuteNonQuery();
            }

            // Ensure columns exist (idempotent)
            using (var cmd = new NpgsqlCommand(@"
                ALTER TABLE savings
                    ADD COLUMN IF NOT EXISTS length TEXT,
                    ADD COLUMN IF NOT EXISTS ""date"" DATE,
                    ADD COLUMN IF NOT EXISTS notes  TEXT;", conn))
            {
                cmd.ExecuteNonQuery();
            }

            // Insert the record
            using (var cmd = new NpgsqlCommand(@"
                INSERT INTO savings (name, amount, length, ""date"", notes)
                VALUES (@n, @a, @l, @d, @not);", conn))
            {
                cmd.Parameters.AddWithValue("@n", NpgsqlDbType.Text, rec.Name);
                cmd.Parameters.AddWithValue("@a", NpgsqlDbType.Numeric, rec.Amount);
                cmd.Parameters.AddWithValue("@l", NpgsqlDbType.Text, (object?)rec.Length ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@d", NpgsqlDbType.Date, rec.Date);
                cmd.Parameters.AddWithValue("@not", NpgsqlDbType.Text, string.IsNullOrWhiteSpace(rec.Notes) ? (object)DBNull.Value : rec.Notes);
                cmd.ExecuteNonQuery();
            }

            // Load back
            var dt = new DataTable();
            using (var cmd = new NpgsqlCommand(
                @"SELECT savings_id, name, amount, ""date"" AS date
                  FROM savings
                  ORDER BY ""date"" DESC;", conn))
            using (var rdr = cmd.ExecuteReader())
                dt.Load(rdr);

            return dt;
        }
    }
}
