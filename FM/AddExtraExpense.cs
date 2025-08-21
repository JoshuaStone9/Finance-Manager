using System;
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

        public AddExtraExpense()
        {
            Text = "Add Extra Expense";
            ClientSize = new Size(560, 540);
            StartPosition = FormStartPosition.CenterScreen;

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
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 7
            };
            cboCategory.Items.AddRange(new object[]
            {
                "Groceries", "Transport", "Entertainment", "Dining Out",
                "Utilities", "Health", "Education", "Gifts", "Home",
                "Personal", "Travel", "Other"
            });

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

            // Frequency (only when Recurring)
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

            // Save
            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(160, 465),
                Width = 120,
                Height = 36,
                TabIndex = 16
            };
            btnSave.Click += BtnSave_Click;

            // View Expenses
            btnViewExpenses = new Button
            {
                Text = "View Expenses",
                Location = new Point(290, 465),
                Width = 200,
                Height = 36,
                TabIndex = 17
            };
            btnViewExpenses.Click += BtnViewExpenses_Click;

            // View All Payments
            btnViewAllPayments = new Button
            {
                Text = "View All Payments",
                Location = new Point(160, 505),
                Width = 200,
                Height = 30,
                TabIndex = 18
            };
            btnViewAllPayments.Click += BtnViewAllPayments_Click;

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
                btnSave, btnViewExpenses, btnViewAllPayments
            });

            // Defaults + events
            cboType.SelectedIndex = 0;              // One-off
            cboCategory.SelectedIndex = 0;
            cboType.SelectedIndexChanged += CboType_SelectedIndexChanged;
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

            var rec = new ExtraExpenseRecord
            {
                Expense_ID = txtExpense_ID.Text.Trim(),
                Name = txtName.Text.Trim(),
                Amount = amount,
                Category = cboCategory.SelectedItem?.ToString() ?? "Other",
                Type = cboType.SelectedItem?.ToString() ?? "One-off",
                Frequency = (cboFrequency.Visible ? (cboFrequency.SelectedItem?.ToString() ?? "Monthly") : "N/A"),
                DateIncurred = dtpDate.Value.Date,
                Notes = txtNotes.Text.Trim()
            };

            ExtraExpenseStore.Expenses.Add(rec);

            MessageBox.Show("Expense saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForNext();
        }

        private void BtnViewExpenses_Click(object? sender, EventArgs e)
        {
            ExtraExpensesRecord expenses = new ExtraExpensesRecord();
            expenses.Show();
        }

        private void BtnViewAllPayments_Click(object? sender, EventArgs e)
        {
            // If you already have an AllPayments form, open it here:
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

            // Unique ID check
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

            if (cboCategory.SelectedIndex < 0)
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

            // Optional rule: avoid future dates
            if (dtpDate.Value.Date > DateTime.Today.AddDays(1))
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
            cboCategory.SelectedIndex = 0;
            cboType.SelectedIndex = 0;
            cboFrequency.SelectedIndex = -1;
            lblFrequency.Visible = cboFrequency.Visible = false;
            dtpDate.Value = DateTime.Today;
            txtNotes.Clear();

            txtExpense_ID.Focus();
        }
    }
}
