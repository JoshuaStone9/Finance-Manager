using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FM
{
    public partial class MainMenu : Form
    {
        
        private Button managePaymentsButton;
        private Button billsButton;
        private Button extraExpensesButton;
        private Button savingsButton;
        private Button investmentsButton;
        private Button allPaymentsButton;
        private Button notificationBell;
        private System.Windows.Forms.Timer closeTimer;
        private Label messageLabel;

        private bool paymentsVisible = false; // track dropdown state
        private bool paymentsNotVisible = true;

        public MainMenu()
        {
            
            InitializeComponent();
            this.Font = new Font("Montserrat", 10, FontStyle.Regular);


        
// Form settings
             this.Text = "Finance Manager";
            this.Size = new Size(500, 500);   // bigger form
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Paint += Form1_Paint; // gradient background

            // Heading
            PictureBox logo = new PictureBox();
            logo.Image = Image.FromFile("images/FM_Logo_Main_Menu.png");
            logo.Size = new Size(180, 180);
            logo.Location = new Point(160, 0);
            logo.SizeMode = PictureBoxSizeMode.StretchImage;
            logo.BackColor = Color.Transparent;

            // Manage Payments Dropdown Button
            managePaymentsButton = new Button();
            managePaymentsButton.Text = "Manage Individual Payments ▼";
            managePaymentsButton.Font = new Font("Montserrat", 10, FontStyle.Regular);
            managePaymentsButton.Size = new Size(280, 45);
            managePaymentsButton.Location = new Point(110, 160); // moved lower
            managePaymentsButton.Click += ManagePaymentsButton_Click;
            managePaymentsButton.BackColor = Color.FromArgb(255, 120, 120);
            // black border
            managePaymentsButton.FlatStyle = FlatStyle.Flat;
            managePaymentsButton.FlatAppearance.BorderColor = Color.Black;
            managePaymentsButton.FlatAppearance.BorderSize = 2;

            // Bills Button
            billsButton = new Button();
            billsButton.Text = "Bills";
            billsButton.Size = new Size(220, 45);
            billsButton.Location = new Point(140, 220); // moved lower
            billsButton.BackColor = Color.FromArgb(255, 150, 150);
            billsButton.Visible = false;
            billsButton.Click += BillsButton_Click;
            // black border
            billsButton.FlatStyle = FlatStyle.Flat;
            billsButton.FlatAppearance.BorderColor = Color.Black;
            billsButton.FlatAppearance.BorderSize = 2;

            // Extra Expenses Button
            extraExpensesButton = new Button();
            extraExpensesButton.Text = "Extra Expenses";
            extraExpensesButton.Size = new Size(220, 45);
            extraExpensesButton.Location = new Point(140, 270); // moved lower
            extraExpensesButton.BackColor = Color.FromArgb(255, 150, 150);
            extraExpensesButton.Visible = false;
            extraExpensesButton.Click += ExtraExpenseButton_Click;
            // black border
            extraExpensesButton.FlatStyle = FlatStyle.Flat;
            extraExpensesButton.FlatAppearance.BorderColor = Color.Black;
            extraExpensesButton.FlatAppearance.BorderSize = 2;

            // Savings Button
            savingsButton = new Button();
            savingsButton.Text = "Savings";
            savingsButton.Size = new Size(220, 45);
            savingsButton.Location = new Point(140, 320); // moved lower
            savingsButton.BackColor = Color.FromArgb(255, 150, 150);
            savingsButton.Visible = false;
            savingsButton.Click += SavingsButton_Click;
            // black border
            savingsButton.FlatStyle = FlatStyle.Flat;
            savingsButton.FlatAppearance.BorderColor = Color.Black;
            savingsButton.FlatAppearance.BorderSize = 2;

            // Investments Button
            investmentsButton = new Button();
            investmentsButton.Text = "Investments";
            investmentsButton.Size = new Size(220, 45);
            investmentsButton.Location = new Point(140, 370); // moved lower
            investmentsButton.BackColor = Color.FromArgb(255, 150, 150);
            investmentsButton.Visible = false;
            investmentsButton.Click += InvestmentsButton_Click;
            // black border
            investmentsButton.FlatStyle = FlatStyle.Flat;
            investmentsButton.FlatAppearance.BorderColor = Color.Black;
            investmentsButton.FlatAppearance.BorderSize = 2;

            // All Payments Button
            allPaymentsButton = new Button();
            allPaymentsButton.Text = "Manage All Payments";
            allPaymentsButton.Size = new Size(280, 45);
            allPaymentsButton.Location = new Point(110, 250); // moved lower
            allPaymentsButton.BackColor = Color.FromArgb(255, 120, 120);
            allPaymentsButton.Visible = true;
            allPaymentsButton.Click += AllPaymentsButton_Click;
            // black border
            allPaymentsButton.FlatStyle = FlatStyle.Flat;
            allPaymentsButton.FlatAppearance.BorderColor = Color.Black;
            allPaymentsButton.FlatAppearance.BorderSize = 2;

            // Add Money Button
            Button addMoneyButton = new Button();
            addMoneyButton.Text = "Add Money";
            addMoneyButton.Size = new Size(280, 45);
            addMoneyButton.Location = new Point(110, 430); // moved lower
            addMoneyButton.BackColor = Color.FromArgb(255, 120, 120);
            addMoneyButton.Click += AddMoneyButton_Click;
            // black border
            addMoneyButton.FlatStyle = FlatStyle.Flat;
            addMoneyButton.FlatAppearance.BorderColor = Color.Black;
            addMoneyButton.FlatAppearance.BorderSize = 2;

            // Load and resize the bell image
            Image notificationBellImg = Image.FromFile("images/NotificationBell_No_Notifications.png");
            Image resized = new Bitmap(notificationBellImg, new Size(70, 70));

            // Create the button
            notificationBell = new Button();
            notificationBell.Size = resized.Size; // button = same size as image
            notificationBell.Location = new Point(10, 10);
            notificationBell.Click += NotificationBell_Click;

            // Make button only show the image
            notificationBell.Image = resized;
            notificationBell.Text = "";
            notificationBell.ImageAlign = ContentAlignment.MiddleCenter;

            // Remove background styling if you want a clean icon look
            notificationBell.FlatStyle = FlatStyle.Flat;
            notificationBell.FlatAppearance.BorderSize = 0;   // no border
            notificationBell.BackColor = Color.Transparent;   // transparent background
            notificationBell.TabStop = false;                 // no focus highlight

            // Initially hide individual payment buttons



            // Add controls to form
            this.Controls.Add(managePaymentsButton);
            this.Controls.Add(billsButton);
            this.Controls.Add(extraExpensesButton);
            this.Controls.Add(savingsButton);
            this.Controls.Add(investmentsButton);
            this.Controls.Add(allPaymentsButton);
            this.Controls.Add(logo);
            this.Controls.Add(notificationBell);
            this.Controls.Add(addMoneyButton);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Create gradient from top to bottom
            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.LightCoral,   // top color
                Color.White,        // bottom color
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
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

        private void SavingsButton_Click(object sender, EventArgs e)
        {
            AddSavings savingsForm = new AddSavings();
            savingsForm.Show();
        }

        private void ExtraExpenseButton_Click(object sender, EventArgs e)
        {
            AddExtraExpense addExtraExpenseForm = new AddExtraExpense();
            addExtraExpenseForm.Show();
        }

        private void AllPaymentsButton_Click(object sender, EventArgs e)
        {
            AllPayments allPaymentsForm = new AllPayments();
            allPaymentsForm.Show();
        }

        private void AddMoneyButton_Click(object sender, EventArgs e)
        {
            AddMoney addMoneyForm = new AddMoney();
            addMoneyForm.Show();
        }

        private void NotificationBell_Click(object sender, EventArgs e)
        {
            // For testing purposes
            Point btnScreenPos = notificationBell.PointToScreen(Point.Empty);
            Point notificationLocation = new Point(
                btnScreenPos.X, btnScreenPos.Y + notificationBell.Height + 5);
            
            NotificationCenter notification = new NotificationCenter("This is a test notification!", 3000, notificationLocation);
            notification.Show();
        }


        private void InitializeComponent()
        {
            // This method is intentionally left empty because you are manually adding controls in the constructor.
            // If you use the designer, this method will be auto-generated.
        }

      
    }
    
}
