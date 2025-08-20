using System;
using System.Linq;
using System.Windows.Forms;

namespace FM
{
    public partial class BillsRecord : Form
    {
        private readonly BindingSource _bs = new();

        public BillsRecord()
        {
            InitializeComponent();
            SetupGrid();

            btnDeleteBills.Click += btnDeleteBills_Click;
            btnCloseBills.Click += (s, e) => Close();
        }

        private void SetupGrid()
        {
            _bs.DataSource = BillStore.Bills;     // BindingList<BillRecord>
            gridBills.AutoGenerateColumns = true;
            gridBills.DataSource = _bs;

            gridBills.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridBills.MultiSelect = true;
            gridBills.ReadOnly = true;
            gridBills.AllowUserToAddRows = false;
            gridBills.AllowUserToDeleteRows = false;
            gridBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnDeleteBills_Click(object? sender, EventArgs e)
        {
            if (gridBills.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select at least one bill to delete.");
                return;
            }

            if (MessageBox.Show($"Delete {gridBills.SelectedRows.Count} bill(s)?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var toRemove = gridBills.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(r => r.DataBoundItem as BillRecord)
                .Where(x => x != null)
                .ToList();

            foreach (var rec in toRemove)
                BillStore.Bills.Remove(rec!);
        }
    }
}
