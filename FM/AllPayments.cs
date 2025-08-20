using System;
using System.Drawing;
using System.Windows.Forms;

namespace FM
{
    public partial class AllPayments : Form
    {
        private readonly BindingSource _bsBills = new();
        private readonly BindingSource _bsInv = new();
        private readonly DataGridView gridBills = new DataGridView { ReadOnly = true };
        private readonly DataGridView gridInvestments = new DataGridView { ReadOnly = true };
        private Label AllPaymentsHeader;

        public AllPayments()
        {
            Text = "All Payments";
            ClientSize = new Size(1000, 900);

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                Padding = new Padding(16, 8, 16, 8)
            };

            AllPaymentsHeader = new Label
            {
                Text = "All Payments",
                Dock = DockStyle.Fill,
                Font = new Font("Cambria", 14F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter
            };

            headerPanel.Controls.Add(AllPaymentsHeader);

            gridBills.Size = new Size(800, 300);
            gridBills.Location = new Point(16, 80);
            gridBills.AllowUserToAddRows = false;
            gridBills.AllowUserToDeleteRows = false;
            gridBills.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            Controls.Add(gridBills);
            Controls.Add(headerPanel);

            BillsDataGrid();

            gridInvestments.Size = new Size(800, 300);
            gridInvestments.Location = new Point(16, 500);
            gridInvestments.AllowUserToAddRows = false;
            gridInvestments.AllowUserToDeleteRows = false;
            gridInvestments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridInvestments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            Controls.Add(gridInvestments);

            InvestmentsDataGrid();
        }

        private void BillsDataGrid()
        {
            _bsBills.DataSource = BillStore.Bills;
            gridBills.AutoGenerateColumns = true;
            gridBills.DataSource = _bsBills;
        }

        private void InvestmentsDataGrid()
        {
            _bsInv.DataSource = InvestmentStore.Investments;
            gridInvestments.AutoGenerateColumns = true;
            gridInvestments.DataSource = _bsInv;
        }
    }
}
