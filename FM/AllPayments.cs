using System;
using System.Drawing;
using System.Globalization;
using System.Linq;            // Sum(...)
using System.Windows.Forms;

namespace FM
{
    public partial class AllPayments : Form
    {
        private readonly BindingSource _bsBills = new();
        private readonly BindingSource _bsInv = new();
        private readonly BindingSource _bsExtExp = new();

        private readonly DataGridView gridBills = new() { ReadOnly = true };
        private readonly DataGridView gridInvestments = new() { ReadOnly = true };
        private readonly DataGridView gridExpenses = new() { ReadOnly = true };

        private TextBox txtGrandTotal;
        private Label lblGrandTotal;

        public AllPayments()
        {
            Text = "All Payments";
            ClientSize = new Size(1000, 620);

            // Header
            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 56, Padding = new Padding(16, 8, 16, 8) };
            var header = new Label { Text = "All Payments", Dock = DockStyle.Fill, Font = new Font("Cambria", 14F), TextAlign = ContentAlignment.MiddleCenter };
            headerPanel.Controls.Add(header);
            Controls.Add(headerPanel);

            // Bills grid
            gridBills.Location = new Point(16, 80);
            gridBills.Size = new Size(960, 220);
            gridBills.AllowUserToAddRows = false;
            gridBills.AllowUserToDeleteRows = false;
            gridBills.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Controls.Add(gridBills);

            // Investments grid
            gridInvestments.Location = new Point(16, 320);
            gridInvestments.Size = new Size(960, 220);
            gridInvestments.AllowUserToAddRows = false;
            gridInvestments.AllowUserToDeleteRows = false;
            gridInvestments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridInvestments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Controls.Add(gridInvestments);

            // Investments grid
            gridExpenses.Location = new Point(16, 660);
            gridExpenses.Size = new Size(960, 220);
            gridExpenses.AllowUserToAddRows = false;
            gridExpenses.AllowUserToDeleteRows = false;
            gridExpenses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridExpenses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Controls.Add(gridExpenses);

            // Total box at bottom-right
            lblGrandTotal = new Label
            {
                Text = "Total:",
                AutoSize = true,
                Location = new Point(700, 560),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            txtGrandTotal = new TextBox
            {
                ReadOnly = true,
                Width = 180,
                Location = new Point(750, 556),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                TextAlign = HorizontalAlignment.Right
            };
            Controls.Add(lblGrandTotal);
            Controls.Add(txtGrandTotal);

            // Bind grids
            BillsDataGrid();
            InvestmentsDataGrid();
            ExtraExpenseDataGrid();

            // Keep total updated whenever lists change
            BillStore.Bills.ListChanged += (_, __) => UpdateTotal();
            InvestmentStore.Investments.ListChanged += (_, __) => UpdateTotal();

            // Initial total
            UpdateTotal();
        }

        private void BillsDataGrid()
        {
            _bsBills.DataSource = BillStore.Bills;         // BindingList<BillRecord>
            gridBills.AutoGenerateColumns = true;
            gridBills.DataSource = _bsBills;
        }

        private void InvestmentsDataGrid()
        {
            _bsInv.DataSource = InvestmentStore.Investments; // BindingList<InvestmentRecord>
            gridInvestments.AutoGenerateColumns = true;
            gridInvestments.DataSource = _bsInv;
        }

        private void ExtraExpenseDataGrid()
        {
            _bsExtExp.DataSource = ExtraExpenseStore.Expenses; // BindingList<InvestmentRecord>
            gridExpenses.AutoGenerateColumns = true;
            gridExpenses.DataSource = _bsExtExp;
        }

        private void UpdateTotal()
        {
            decimal total =
                BillStore.Bills.Sum(b => b.Amount) +
                InvestmentStore.Investments.Sum(i => i.Amount);

            txtGrandTotal.Text = total.ToString("C", CultureInfo.CurrentCulture);
        }
    }
}
