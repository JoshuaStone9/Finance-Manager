using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
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
        private Panel bottomPanel;

        public AllPayments()
        {
            Text = "All Payments";
            ClientSize = new Size(1200, 820); // a bit wider for comfort

            // Header
            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 56, Padding = new Padding(16, 8, 16, 8) };
            var header = new Label { Text = "All Payments", Dock = DockStyle.Fill, Font = new Font("Cambria", 14F), TextAlign = ContentAlignment.MiddleCenter };
            headerPanel.Controls.Add(header);
            Controls.Add(headerPanel);

            // Bills grid
            gridBills.Location = new Point(16, 80);
            gridBills.Size = new Size(1168, 200);
            gridBills.AllowUserToAddRows = false;
            gridBills.AllowUserToDeleteRows = false;
            gridBills.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Controls.Add(gridBills);

            // Investments grid
            gridInvestments.Location = new Point(16, 300);
            gridInvestments.Size = new Size(1168, 200);
            gridInvestments.AllowUserToAddRows = false;
            gridInvestments.AllowUserToDeleteRows = false;
            gridInvestments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridInvestments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Controls.Add(gridInvestments);

            // Expenses grid (moved up so bottom panel is visible)
            gridExpenses.Location = new Point(16, 520);
            gridExpenses.Size = new Size(1168, 180);
            gridExpenses.AllowUserToAddRows = false;
            gridExpenses.AllowUserToDeleteRows = false;
            gridExpenses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridExpenses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Controls.Add(gridExpenses);

            // Bottom panel for Total
            bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                Padding = new Padding(12)
            };

            // Right-aligned layout for total
            var totalLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                WrapContents = false,
                Padding = new Padding(0, 10, 0, 0),
                Margin = new Padding(0)
            };

            txtGrandTotal = new TextBox
            {
                ReadOnly = true,
                Width = 180,
                TextAlign = HorizontalAlignment.Right,
                Font = new Font("Segoe UI", 11F),
                Margin = new Padding(8, 0, 0, 0)
            };

            lblGrandTotal = new Label
            {
                Text = "Total:",
                AutoSize = true,
                Font = new Font("Segoe UI", 11F),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 4, 0, 0),
                Margin = new Padding(0, 0, 8, 0)
            };

            totalLayout.Controls.Add(txtGrandTotal);
            totalLayout.Controls.Add(lblGrandTotal);

            bottomPanel.Controls.Add(totalLayout);
            Controls.Add(bottomPanel);

            // Bind grids
            BillsDataGrid();
            InvestmentsDataGrid();
            ExtraExpenseDataGrid();

            // Keep total updated whenever lists change
            BillStore.Bills.ListChanged += (_, __) => UpdateTotal();
            InvestmentStore.Investments.ListChanged += (_, __) => UpdateTotal();
            _bsExtExp.ListChanged += (_, __) => UpdateTotal();

            // Initial total
            UpdateTotal();
        }

        private void BillsDataGrid()
        {
            _bsBills.DataSource = BillStore.Bills;
            gridBills.AutoGenerateColumns = true;
            gridBills.DataSource = _bsBills;

            // Apply £ formatting AFTER binding
            if (gridBills.Columns["Amount"] != null)
            {
                gridBills.Columns["Amount"].DefaultCellStyle.Format = "C";
                gridBills.Columns["Amount"].DefaultCellStyle.FormatProvider = new CultureInfo("en-GB");
            }
        }

        private void InvestmentsDataGrid()
        {
            _bsInv.DataSource = InvestmentStore.Investments;
            gridInvestments.AutoGenerateColumns = true;
            gridInvestments.DataSource = _bsInv;

            if (gridInvestments.Columns["Amount"] != null)
            {
                gridInvestments.Columns["Amount"].DefaultCellStyle.Format = "C";
                gridInvestments.Columns["Amount"].DefaultCellStyle.FormatProvider = new CultureInfo("en-GB");
            }
        }


        private void ExtraExpenseDataGrid()
        {
            _bsExtExp.DataSource = ExtraExpenseStore.Expenses;
            gridExpenses.AutoGenerateColumns = true;
            gridExpenses.DataSource = _bsExtExp;

            if (gridExpenses.Columns["Amount"] != null)
            {
                gridExpenses.Columns["Amount"].DefaultCellStyle.Format = "C";
                gridExpenses.Columns["Amount"].DefaultCellStyle.FormatProvider = new CultureInfo("en-GB");
            }
        }
        private void UpdateTotal()
        {
            decimal total =
                BillStore.Bills.Sum(b => b.Amount) +
                InvestmentStore.Investments.Sum(i =>
                {
                    decimal val;
                    return decimal.TryParse(i.Amount, out val) ? val : 0m;
                }) +
                ExtraExpenseStore.Expenses.Sum(e => e.Amount);

            txtGrandTotal.Text = total.ToString("C", CultureInfo.CurrentCulture);
        }
    }
}
