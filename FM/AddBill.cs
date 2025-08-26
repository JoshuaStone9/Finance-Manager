using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
//using Npgsql;
using System.Data;

namespace FM
{
    public partial class AddBill : Form
    {
        private Label lblAddBill;

        private Label lblBillId;
        private TextBox txtBillId;

        private Label lblName;
        private TextBox txtName;

        private Label lblAmount;
        private Label lblPound;
        private TextBox txtAmount;

        private Label lblType;
        private ComboBox cboType;

        private Label lblLength;
        private ComboBox cboLength;

        private Label lblDueDate;
        private DateTimePicker dtpDueDate;

        private Label lblDescription;
        private TextBox txtDescription;

        private Button btnSave;
        private Button btnViewBills;
        private Button btnMainMenu;
        private Button btnViewAllPayments;

        private Button btnAddInvestment;

        // Bottom container to keep buttons visible on any DPI
        private Panel bottomPanel;

        public AddBill()
        {
            // ---- Form ----
            Text = "Add Bill";
            ClientSize = new Size(520, 560); // a bit taller for comfortable spacing
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;

            SuspendLayout();

            lblAddBill = new Label
            {
                Text = "Add Bill",
                Location = new Point(220, 10),
                AutoSize = true,
                Font = new Font("Cambria", 14F, FontStyle.Regular)
            };

            // Bill ID
            lblBillId = new Label { Text = "Bill ID", Location = new Point(20, 60), AutoSize = true, TabIndex = 0 };
            txtBillId = new TextBox { Location = new Point(160, 56), Width = 160, TabIndex = 1 };
            txtBillId.Text = IdGenerator.GetNextId().ToString();
            txtBillId.ReadOnly = true;

            // Name
            lblName = new Label { Text = "Name", Location = new Point(20, 100), AutoSize = true, TabIndex = 2 };
            txtName = new TextBox { Location = new Point(160, 96), Width = 300, TabIndex = 3 };

            // Amount
            lblAmount = new Label { Text = "Amount", Location = new Point(20, 140), AutoSize = true, TabIndex = 4 };
            lblPound = new Label { Text = "£", Location = new Point(160, 140), AutoSize = true };
            txtAmount = new TextBox { Location = new Point(175, 136), Width = 120, TabIndex = 5 };

            // Type
            lblType = new Label { Text = "Type", Location = new Point(20, 180), AutoSize = true, TabIndex = 6 };
            cboType = new ComboBox
            {
                Location = new Point(160, 176),
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 7
            };
            cboType.Items.AddRange(new object[] { "Permanent", "Temporary" });

            // Length Of Time (hidden unless Temporary)
            lblLength = new Label
            {
                Text = "Length Of Time",
                Location = new Point(20, 220),
                AutoSize = true,
                TabIndex = 8,
                Visible = false
            };

            cboLength = new ComboBox
            {
                Location = new Point(160, 216),
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 9,
                Visible = false
            };
            cboLength.Items.Add("Not Sure");
            for (int m = 1; m <= 12; m++)
                cboLength.Items.Add(m == 1 ? "1 Month" : $"{m} Months");
            cboLength.SelectedIndex = 0;

            // Due Date
            lblDueDate = new Label { Text = "Due Date", Location = new Point(20, 260), AutoSize = true, TabIndex = 10 };
            dtpDueDate = new DateTimePicker
            {
                Location = new Point(160, 256),
                Width = 200,
                TabIndex = 11,
                Format = DateTimePickerFormat.Short
            };

            // Description
            lblDescription = new Label { Text = "Description", Location = new Point(20, 300), AutoSize = true, TabIndex = 12 };
            txtDescription = new TextBox
            {
                Location = new Point(160, 296),
                Width = 300,
                Height = 140,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                TabIndex = 13
            };

            // ---- Bottom button panel (always visible) ----
            bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 110
            };

            btnAddInvestment = new Button
            {
                Text = "Add Investment",
                Location = new Point(20, 15),
                Width = 120,
                Height = 35,
                TabIndex = 16
            };
            btnAddInvestment.Click += BtnAddInvestment_Click;

            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(160, 15),
                Width = 120,
                Height = 35,
                TabIndex = 14
            };
            btnSave.Click += BtnSave_Click;

            btnViewBills = new Button
            {
                Text = "View Bills",
                Location = new Point(300, 15),
                Width = 160,
                Height = 35,
                TabIndex = 15
            };
            btnViewBills.Click += BtnViewBills_Click;

            btnMainMenu = new Button
            {
                Text = "Main Menu",
                Location = new Point(160, 60),
                Width = 120,
                Height = 35,
                TabIndex = 17
            };
            btnMainMenu.Click += BtnMainMenu_Click;

            btnViewAllPayments = new Button
            {
                Text = "View All Payments",
                Location = new Point(300, 60),
                Width = 160,
                Height = 35,
                TabIndex = 18
            };
            btnViewAllPayments.Click += BtnViewAllPayments_Click;

            bottomPanel.Controls.AddRange(new Control[]
            {
                btnAddInvestment, btnSave, btnViewBills, btnMainMenu, btnViewAllPayments
            });

            // ---- Add controls to form ----
            Controls.AddRange(new Control[]
            {
                lblAddBill,
                lblBillId, txtBillId,
                lblName, txtName,
                lblAmount, lblPound, txtAmount,
                lblType, cboType,
                lblLength, cboLength,
                lblDueDate, dtpDueDate,
                lblDescription, txtDescription,
                bottomPanel // add last so it docks correctly
            });

            // Defaults + events
            cboType.SelectedIndex = 0; // default Permanent
            CboType_SelectedIndexChanged(this, EventArgs.Empty);
            cboType.SelectedIndexChanged += CboType_SelectedIndexChanged;

            ResumeLayout(false);
        }

        private void CboType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            bool isTemporary = string.Equals(
                cboType.SelectedItem?.ToString(),
                "Temporary",
                StringComparison.OrdinalIgnoreCase);

            lblLength.Visible = isTemporary;
            cboLength.Visible = isTemporary;

            if (!isTemporary)
                cboLength.SelectedIndex = 0;
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

            var record = new BillRecord
            {
                BillId = txtBillId.Text.Trim(),
                Name = txtName.Text.Trim(),
                Amount = amount,
                Type = cboType.SelectedItem?.ToString() ?? "Permanent",
                Length = (cboLength.Visible ? (cboLength.SelectedItem?.ToString() ?? "Not Sure") : "Not Applicable"),
                DueDate = dtpDueDate.Value.Date,
                Description = txtDescription.Text.Trim()
            };

            BillStore.Bills.Add(record);

            MessageBox.Show("Bill saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearFormForNext();
        }

        private void BtnViewBills_Click(object? sender, EventArgs e)
        {
            var manageBills = new BillsRecord();
            manageBills.Show();
        }

        private void BtnMainMenu_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnViewAllPayments_Click(object? sender, EventArgs e)
        {
            var allPayments = new AllPayments();
            allPayments.Show();
        }

        private void BtnAddInvestment_Click(object? sender, EventArgs e)
        {
            var addInvestment = new AddInvestment();
            addInvestment.Show();
        }

        // ===== Helpers =====

        public static class IdGenerator
        {
         
            private static int _nextId = 1;
            
            public static int GetNextId()
            {
                return _nextId++;
            }
  
        }

        private string? ValidateInputs(out decimal amount)
        {
            amount = 0m;


            var exists = BillStore.Bills.Any(b =>
                string.Equals(b.BillId, txtBillId.Text.Trim(), StringComparison.OrdinalIgnoreCase));
            if (exists)
            {
                txtBillId.BackColor = Color.MistyRose;
                return "That Bill ID already exists. Please use a unique ID.";
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

            if (dtpDueDate.Value.Date < DateTime.Today.AddDays(-1))
            {
                dtpDueDate.CalendarMonthBackground = Color.MistyRose;
                return "Due Date cannot be in the past.";
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
            txtBillId.BackColor = SystemColors.Window;
            txtName.BackColor = SystemColors.Window;
            txtAmount.BackColor = SystemColors.Window;
            cboLength.BackColor = SystemColors.Window;
            dtpDueDate.CalendarMonthBackground = SystemColors.Window;
        }

        private void ClearFormForNext()
        {
            txtBillId.Clear();
            txtName.Clear();
            txtAmount.Clear();
            cboType.SelectedIndex = 0;
            cboLength.SelectedIndex = 0;
            dtpDueDate.Value = DateTime.Today;
            txtDescription.Clear();
            txtBillId.Focus();
        }

        // Optional getters
        public string EnteredBillId => txtBillId.Text.Trim();
        public string EnteredName => txtName.Text.Trim();
        public decimal EnteredAmount => decimal.TryParse(txtAmount.Text, out var v) ? v : 0m;
        public bool IsTemporary => string.Equals(cboType.SelectedItem?.ToString(), "Temporary", StringComparison.OrdinalIgnoreCase);
        public string SelectedLength => cboLength.Visible ? cboLength.SelectedItem?.ToString() ?? "Not Sure" : "Not Applicable";
        public DateTime DueDate => dtpDueDate.Value.Date;
        public string DescriptionText => txtDescription.Text.Trim();

//        private void CreateAndLoadBillsTable()
//        {
//            string cs = "Host=localhost;Database=Finance_Manager;Username=admin;Password=banana001;SslMode=Disable";

//            using var conn = new NpgsqlConnection(cs);
//            conn.Open();

//            // Create tables if not exist
//            using (var cmd = new NpgsqlCommand(@"
//CREATE TABLE IF NOT EXISTS bills(
//  billid SERIAL PRIMARY KEY,
//  name TEXT NOT NULL,
//  amount NUMERIC(12,2) NOT NULL,
//  type TEXT,
//  length TEXT,
//  duedate DATE,
//  description TEXT
//);", conn))
//            {
//                cmd.ExecuteNonQuery();
//            }

//            // Insert
//            using (var cmd = new NpgsqlCommand(
//                "INSERT INTO bills(name,amount,type,length,duedate,description) VALUES(@n,@a,@t,@l,@d,@desc)", conn))
//            {
//                cmd.Parameters.AddWithValue("@n", "Electric");
//                cmd.Parameters.AddWithValue("@a", 64.99m);
//                cmd.Parameters.AddWithValue("@t", "Permanent");
//                cmd.Parameters.AddWithValue("@l", (object)DBNull.Value);
//                cmd.Parameters.AddWithValue("@d", DateTime.Today);
//                cmd.Parameters.AddWithValue("@desc", "Monthly");
//                cmd.ExecuteNonQuery();
//            }

//            // Load into DataTable (bind to DataGridView)
//            var dt = new DataTable();
//            using (var cmd = new NpgsqlCommand("SELECT billid,name,amount,duedate FROM bills ORDER BY duedate DESC", conn))
//            using (var rdr = cmd.ExecuteReader()) dt.Load(rdr);

//            // e.g. gridBills.DataSource = dt;
//        }
    }
}
