using System;
using System.Linq;
using System.Windows.Forms;

namespace FM
{
    public partial class InvestmentsRecord : Form
    {
        private readonly BindingSource _bs = new();

        public InvestmentsRecord()
        {
            InitializeComponent();
            SetupGrid();

            btnDeleteInvestments.Click += btnDeleteInvestments_Click;
            btnCloseInvestments.Click += (s, e) => Close();
        }

        private void SetupGrid()
        {
            _bs.DataSource = InvestmentStore.Investments; // BindingList<InvestmentRecord>
            gridInvestments.AutoGenerateColumns = true;
            gridInvestments.DataSource = _bs;

            gridInvestments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridInvestments.MultiSelect = true;
            gridInvestments.ReadOnly = true;
            gridInvestments.AllowUserToAddRows = false;
            gridInvestments.AllowUserToDeleteRows = false;
            gridInvestments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnDeleteInvestments_Click(object? sender, EventArgs e)
        {
            if (gridInvestments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select at least one investment to delete.");
                return;
            }

            if (MessageBox.Show($"Delete {gridInvestments.SelectedRows.Count} investment(s)?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var toRemove = gridInvestments.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(r => r.DataBoundItem as InvestmentRecord)
                .Where(x => x != null)
                .ToList();

            foreach (var rec in toRemove)
                InvestmentStore.Investments.Remove(rec!);
        }
    }
}
