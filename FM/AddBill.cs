using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Npgsql;

namespace FM
{
    public partial class AddBill : Form
    {
        private Label lblAddBill;

        private Label lblName;
        private TextBox txtName;

        private Label lblAmount;
        private Label lblPound;
        private TextBox txtAmount;

        private Label lblType;
        private ComboBox cboType;

        private Label lblLength;
        private ComboBox cboLength;

        private Label lblDate;
        private DateTimePicker dtpDate;

        private Label lblDescription;
        private TextBox txtDescription;

        private Button btnSave;
        private Button btnViewBills;
        private Button btnMainMenu;
        private Button btnViewAllPayments;

        private Panel bottomPanel;
        private PictureBox logo;

        public AddBill()
        {
            // --- Form ---
            Text = "Add Bill";
            ClientSize = new Size(520, 620); // taller so bottom buttons are visible
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;

            this.Font = new Font("Montserrat", 10, FontStyle.Regular);
            this.Paint += AddBill_Paint;

            SuspendLayout();

            // optional logo
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
            lblAddBill = new Label
            {
                Text = "Add Bill",
                Location = new Point((ClientSize.Width - 100) / 2, 120),
                AutoSize = true,
                Font = new Font("Montserrat", 14F, FontStyle.Bold),
                BackColor = Color.Transparent
            };

            // Name
            lblName = new Label { Text = "Name", Location = new Point(20, 170), AutoSize = true, BackColor = Color.Transparent, TabIndex = 0 };
            txtName = new TextBox { Location = new Point(160, 166), Width = 300, TabIndex = 1 };

            // Amount
            lblAmount = new Label { Text = "Amount", Location = new Point(20, 210), AutoSize = true, BackColor = Color.Transparent, TabIndex = 2 };
            lblPound = new Label { Text = "�", Location = new Point(160, 210), AutoSize = true, BackColor = Color.Transparent };
            txtAmount = new TextBox { Location = new Point(175, 206), Width = 120, TabIndex = 3 };

            // Type
            lblType = new Label { Text = "Type", Location = new Point(20, 250), AutoSize = true, BackColor = Color.Transparent, TabIndex = 4 };
            cboType = new ComboBox
            {
                Location = new Point(160, 246),
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 5
            };
            cboType.Items.AddRange(new object[] { "Permanent", "Temporary" });
            cboType.SelectedIndex = 0;

            // Length
            lblLength = new Label
            {
                Text = "Length Of Time",
                Location = new Point(20, 290),
                AutoSize = true,
                BackColor = Color.Transparent,
                TabIndex = 6,
                Visible = false
            };

            cboLength = new ComboBox
            {
                Location = new Point(160, 286),
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 7,
                Visible = false
            };
            cboLength.Items.Add("Not Sure");
            for (int m = 1; m <= 12; m++)
                cboLength.Items.Add(m == 1 ? "1 Month" : $"{m} Months");
            cboLength.SelectedIndex = 0;

            // Date
            lblDate = new Label { Text = "Date", Location = new Point(20, 330), AutoSize = true, BackColor = Color.Transparent, TabIndex = 8 };
            dtpDate = new DateTimePicker
            {
                Location = new Point(160, 326),
                Width = 200,
                TabIndex = 9,
                Format = DateTimePickerFormat.Short
            };

            // Description
            lblDescription = new Label { Text = "Description", Location = new Point(20, 370), AutoSize = true, BackColor = Color.Transparent, TabIndex = 10 };
            txtDescription = new TextBox
            {
                Location = new Point(160, 366),
                Width = 300,
                Height = 150,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                TabIndex = 11
            };

            // Bottom panel
            bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = Color.Transparent
            };

            btnSave = MakePrimaryButton("Save", new Point(20, 20), 150, 40, 12, BtnSave_Click);
            btnViewBills = MakeSecondaryButton("View Bills", new Point(180, 20), 150, 40, 13, BtnViewBills_Click);
            btnMainMenu = MakeSecondaryButton("Main Menu", new Point(340, 20), 150, 40, 14, BtnMainMenu_Click);
            btnViewAllPayments = MakeSecondaryButton("View All Payments", new Point(180, 70), 200, 40, 15, BtnViewAllPayments_Click);

            bottomPanel.Controls.AddRange(new Control[] { btnSave, btnViewBills, btnMainMenu, btnViewAllPayments });

            Controls.AddRange(new Control[]
            {
                lblAddBill,
                lblName, txtName,
                lblAmount, lblPound, txtAmount,
                lblType, cboType,
                lblLength, cboLength,
                lblDate, dtpDate,
                lblDescription, txtDescription,
                bottomPanel
            });

            cboType.SelectedIndexChanged += CboType_SelectedIndexChanged;

            ResumeLayout(false);
        }

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

        private void AddBill_Paint(object? sender, PaintEventArgs e)
        {
            using (var brush = new LinearGradientBrush(this.ClientRectangle, Color.LightCoral, Color.White, LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void CboType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            bool isTemporary = string.Equals(cboType.SelectedItem?.ToString(), "Temporary", StringComparison.OrdinalIgnoreCase);

            lblLength.Visible = isTemporary;
            cboLength.Visible = isTemporary;

            if (!isTemporary)
                cboLength.SelectedIndex = 0;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            CreateAndLoadBillsTable();
            ResetFieldBackColors();

            string? error = ValidateInputs(out decimal amount);
            if (error != null)
            {
                MessageBox.Show(error, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var record = new BillRecord
            {
                Name = txtName.Text.Trim(),
                Amount = amount,
                Type = cboType.SelectedItem?.ToString() ?? "Permanent",
                Length = (cboLength.Visible ? (cboLength.SelectedItem?.ToString() ?? "Not Sure") : "Not Applicable"),
                DueDate = dtpDate.Value.Date,  
                Description = txtDescription.Text.Trim()
            };

            BillStore.Bills.Add(record);

            MessageBox.Show("Bill saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearFormForNext();
        }

        private void BtnViewBills_Click(object? sender, EventArgs e) => new BillsRecord().Show();
        private void BtnMainMenu_Click(object? sender, EventArgs e) => this.Close();
        private void BtnViewAllPayments_Click(object? sender, EventArgs e) => new AllPayments().Show();

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

            if (dtpDate.Value.Date < DateTime.Today.AddDays(-1))
            {
                dtpDate.CalendarMonthBackground = Color.MistyRose;
                return "Date cannot be in the past.";
            }

            if (cboType.SelectedItem?.ToString() == "Temporary" && cboLength.SelectedIndex < 0)
            {
                cboLength.BackColor = Color.MistyRose;
                return "Please select the Length Of Time for a Temporary bill.";
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
            cboType.SelectedIndex = 0;
            cboLength.SelectedIndex = 0;
            dtpDate.Value = DateTime.Today;
            txtDescription.Clear();
            txtName.Focus();
        }

        private void CreateAndLoadBillsTable()
        {
            string cs = "Host=localhost;Database=Finance_Manager;Username=postgres;Password=banana001;SslMode=Disable";

            using var conn = new NpgsqlConnection(cs);
            conn.Open();

            using (var cmd = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS bills(
                billid SERIAL PRIMARY KEY,
                name TEXT NOT NULL,
                amount NUMERIC(12,2) NOT NULL,
                type TEXT,
                length TEXT,
                date DATE,
                description TEXT
                );", conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new NpgsqlCommand(
                "INSERT INTO bills(name,amount,type,length,date,description) VALUES(@n,@a,@t,@l,@d,@desc)", conn))
            {
                cmd.Parameters.AddWithValue("@n", txtName.Text);
                cmd.Parameters.AddWithValue("@a", decimal.Parse(txtAmount.Text));
                cmd.Parameters.AddWithValue("@t", cboType.SelectedItem?.ToString() ?? "Unknown");
                cmd.Parameters.AddWithValue("@l", cboLength.Visible ? (cboLength.SelectedItem?.ToString() ?? "Not Sure") : "Not Applicable");
                cmd.Parameters.AddWithValue("@d", dtpDate.Value.Date);
                cmd.Parameters.AddWithValue("@desc", txtDescription.Text);
                cmd.ExecuteNonQuery();
            }

            var dt = new DataTable();
            using (var cmd = new NpgsqlCommand("SELECT billid,name,amount,date FROM bills ORDER BY date DESC", conn))
            using (var rdr = cmd.ExecuteReader())
                dt.Load(rdr);
        }
    }
}
