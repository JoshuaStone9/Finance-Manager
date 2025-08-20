using System;
using System.Drawing;
using System.Windows.Forms;

namespace FM
{
    public partial class AllPayments : Form
    {
        private readonly BindingSource _bs = new BindingSource();
        private readonly DataGridView _grid = new DataGridView { ReadOnly = true };
        private Label AllPaymentsHeader;

        public AllPayments()
        {
            Text = "All Payments";
            ClientSize = new Size(1000, 600);   

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

            
            _grid.Dock = DockStyle.Fill;         
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            
            Controls.Add(_grid);
            Controls.Add(headerPanel);

            BillsDataGrid();
        }

        private void BillsDataGrid()
        {

            _bs.DataSource = BillStore.Bills;    
            _grid.AutoGenerateColumns = true;    
            _grid.DataSource = _bs;
        }


    }
}
