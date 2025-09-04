using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

       
        public AddSavings()
        {
            InitializeComponent();

            Text = "Add Savings";
            ClientSize = new Size(560, 540);
            StartPosition = FormStartPosition.CenterScreen;

            bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 100 };

            lblTitle = new Label
            {
                Text = "Add Savings",
                Location = new Point(190, 12),
                AutoSize = true,
                Font = new Font("Cambria", 14F, FontStyle.Regular)
            };
            

            
            lblName = new Label { Text = "Name", Location = new Point(20, 100), AutoSize = true, TabIndex = 2 };
            txtName = new TextBox { Location = new Point(160, 96), Width = 330, TabIndex = 3 };

            
            lblAmount = new Label { Text = "Amount", Location = new Point(20, 140), AutoSize = true, TabIndex = 4 };
            lblPound = new Label { Text = "£", Location = new Point(160, 140), AutoSize = true };
            txtAmount = new TextBox { Location = new Point(175, 136), Width = 120, TabIndex = 5 };


            
            lblLength = new Label
            {
                Text = "Length",
                Location = new Point(20, 260),
                AutoSize = true,
                TabIndex = 10
            };
            cboLength = new ComboBox
            {
                Location = new Point(160, 256),
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 11
            };
            cboLength.Items.AddRange(new object[] { "Daily", "Weekly", "Monthly", "Quarterly", "Yearly" });


            lblDate = new Label { Text = "Date", Location = new Point(20, 300), AutoSize = true, TabIndex = 12 };
            dtpDate = new DateTimePicker
            {
                Location = new Point(160, 296),
                Width = 200,
                Format = DateTimePickerFormat.Short,
                TabIndex = 13
            };

            
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

            btnViewSavings = new Button { Text = "View Expenses", Location = new Point(300, 15), Width = 200, Height = 36, TabIndex = 17 };
            btnViewSavings.Click += BtnViewSavings_Click;

            btnViewAllPayments = new Button { Text = "View All Payments", Location = new Point(300, 60), Width = 200, Height = 35, TabIndex = 18 };
            btnViewAllPayments.Click += BtnViewAllPayments_Click;

            btnMainMenu = new Button { Text = "Main Menu", Location = new Point(160, 60), Width = 120, Height = 35, TabIndex = 19 };
            btnMainMenu.Click += BtnMainMenu_Click;

            bottomPanel.Controls.AddRange(new Control[] { btnSave, btnViewSavings, btnMainMenu, btnViewAllPayments });

            Controls.AddRange(new Control[]
            {
                lblTitle,
                lblName, txtName,
                lblAmount, lblPound, txtAmount,
                lblLength, cboLength,
                lblDate, dtpDate,
                lblNotes, txtNotes,
                bottomPanel,
            });

           
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

        // Fix: Pass required arguments to InsertSavingsToDb

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
                Length = (cboLength.Visible ? (cboLength.SelectedItem?.ToString() ?? "Monthly") : "N/A"),
                Date = dtpDate.Value.Date,
                Notes = txtNotes.Text.Trim()
            };

            try
            {
                EnsureSchemaAndSeedCategories(); // idempotent

                // Provide dummy values for categoryId and categoryName since they are not used in InsertSavingsToDb
                InsertSavingsToDb(rec, 0, string.Empty);

                SavingsStore.Savings.Add(rec);

                MessageBox.Show("Expense saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not save expense:\n{ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnViewSavings_Click(object? sender, EventArgs e)
        {
            var savings = new SavingsRecord();
            //savings.Show();
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
                return "Date Incurred cannot be in the future.";
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

       
        private void EnsureSchemaAndSeedCategories()
        {
            using var conn = new NpgsqlConnection(ConnStr);
            conn.Open();

            // 1) Create base tables if missing
            using (var cmd = new NpgsqlCommand(@"

                CREATE TABLE IF NOT EXISTS savings(
                    savings_id SERIAL PRIMARY KEY,
                    name TEXT NOT NULL,
                    amount NUMERIC(12,2) NOT NULL
                );", conn))
            {
                cmd.ExecuteNonQuery();
            }

            // 2) Add/ensure columns (idempotent)
            using (var cmd = new Npgsql.NpgsqlCommand(@"
                ALTER TABLE savings ADD COLUMN IF NOT EXISTS length      TEXT;
                ALTER TABLE savings ADD COLUMN IF NOT EXISTS date     DATE;
                ALTER TABLE savings ADD COLUMN IF NOT EXISTS notes TEXT;
            ", conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

             private void InsertSavingsToDb(SavingsRecord rec, int categoryId, string categoryName)
        {
            using var conn = new NpgsqlConnection(ConnStr);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
                INSERT INTO savings
                    (name, amount, length, date, notes)
                VALUES
                    (@n,   @a,  @l,  @d,  @not);", conn);

            cmd.Parameters.AddWithValue("@n", NpgsqlDbType.Text, rec.Name);
            cmd.Parameters.AddWithValue("@a", NpgsqlDbType.Numeric, rec.Amount);
            cmd.Parameters.AddWithValue("@l", NpgsqlDbType.Text, rec.Length);
            cmd.Parameters.AddWithValue("@d", NpgsqlDbType.Date, rec.Date);
            cmd.Parameters.AddWithValue("@not", NpgsqlDbType.Text, rec.Notes);

            cmd.ExecuteNonQuery();
        }
    }
}
        
