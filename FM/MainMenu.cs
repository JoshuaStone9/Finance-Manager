using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FM
{
    public partial class MainMenu : Form
    {
        private Label headingLabel;
        private Button managePaymentsButton;
        private Button billsButton;
        private Button extraExpensesButton;
        private Button savingsButton;
        private Button investmentsButton;
        private Button allPaymentsButton;

        private bool paymentsVisible = false; // track dropdown state
        private bool paymentsNotVisible = true;

        public MainMenu()
        {
            InitializeComponent();

            // Form settings
            this.Text = "Finance Manager";
            this.Size = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Heading
            headingLabel = new Label();
            headingLabel.Text = "Finance Manager";
            headingLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            headingLabel.AutoSize = true;
            headingLabel.Location = new Point(120, 20);

            // Manage Payments Dropdown Button
            managePaymentsButton = new Button();
            managePaymentsButton.Text = "Manage Individual Payments ▼";
            managePaymentsButton.Size = new Size(250, 40);
            managePaymentsButton.Location = new Point(75, 70);
            managePaymentsButton.Click += ManagePaymentsButton_Click;

            // Bills Button
            billsButton = new Button();
            billsButton.Text = "Bills";
            billsButton.Size = new Size(200, 40);
            billsButton.Location = new Point(100, 120);
            billsButton.Visible = false; // hidden by default

            billsButton.Click += BillsButton_Click;
            // Extra Expenses Button
            extraExpensesButton = new Button();
            extraExpensesButton.Text = "Extra Expenses";
            extraExpensesButton.Size = new Size(200, 40);
            extraExpensesButton.Location = new Point(100, 170);
            extraExpensesButton.Visible = false;

            // Savings Button
            savingsButton = new Button();
            savingsButton.Text = "Savings";
            savingsButton.Size = new Size(200, 40);
            savingsButton.Location = new Point(100, 220);
            savingsButton.Visible = false;

            // Investments Button
            investmentsButton = new Button();
            investmentsButton.Text = "Investments";
            investmentsButton.Size = new Size(200, 40);
            investmentsButton.Location = new Point(100, 270);
            investmentsButton.Visible = false;
            investmentsButton.Click += InvestmentsButton_Click;

            // All Payments Button
            allPaymentsButton = new Button();
            allPaymentsButton.Text = "Manage All Payments";
            allPaymentsButton.Size = new Size(200, 40);
            allPaymentsButton.Location = new Point(100, 200);
            allPaymentsButton.Visible = true;
            allPaymentsButton.Click += AllPaymentsButton_Click;

            // Add controls to form
            this.Controls.Add(headingLabel);
            this.Controls.Add(managePaymentsButton);
            this.Controls.Add(billsButton);
            this.Controls.Add(extraExpensesButton);
            this.Controls.Add(savingsButton);
            this.Controls.Add(investmentsButton);
            this.Controls.Add(allPaymentsButton);
        }

        private void ManagePaymentsButton_Click(object sender, EventArgs e)
        {
            paymentsVisible = !paymentsVisible;
            paymentsNotVisible = ! paymentsNotVisible;

            billsButton.Visible = paymentsVisible;
            extraExpensesButton.Visible = paymentsVisible;
            savingsButton.Visible = paymentsVisible;
            investmentsButton.Visible = paymentsVisible;
            allPaymentsButton.Visible = paymentsNotVisible;

            // Update arrow icon for dropdown effect
            managePaymentsButton.Text = paymentsVisible
                ? "Manage Individual Payments ▲"
                : "Manage Individual Payments ▼";
        }

        private void BillsButton_Click(object sender, EventArgs e)
        {
            AddBill billForm = new AddBill();
            billForm.Show();
        }

        private void InvestmentsButton_Click(object sender, EventArgs e)
        {
            AddInvestment investForm = new AddInvestment();
            investForm.Show();
        }

        private void AllPaymentsButton_Click(object sender, EventArgs e)
        {
            AllPayments allPaymentsForm = new AllPayments();
            allPaymentsForm.Show();
        }


        private void InitializeComponent()
        {
            // This method is intentionally left empty because you are manually adding controls in the constructor.
            // If you use the designer, this method will be auto-generated.
        }
    }
    
}
