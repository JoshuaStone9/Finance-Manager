using System;
using System.Windows.Forms;

namespace FM
{
    public partial class AddInvestment : System.Windows.Forms.Form
    {
        public AddInvestment()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            // TODO: Persist this to your InvestmentStore / DB.
            // Example shape you can adapt:
            // var rec = new InvestmentRecord
            // {
            //     Name = txtName.Text,
            //     Amount = numAmount.Value,
            //     Date = dtDate.Value,
            //     Category = cbCategory.SelectedItem?.ToString() ?? "",
            //     Length = cbLength.SelectedItem?.ToString() ?? "",
            //     Notes = txtNotes.Text
            // };
            // InvestmentStore.Add(rec);
            // Close or clear form as desired.
        }

        private void btnViewBills_Click(object? sender, EventArgs e)
        {
            InvestmentsRecord investements = new InvestmentsRecord();
            investements.Show();
        }

        private void btnViewAllPayments_Click(object? sender, EventArgs e)
        {
            // Optional: navigate to your Investments list screen.
            // new InvestmentsRecord().Show(this);
        }
    }
}
