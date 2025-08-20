using System.Drawing;
using System.Windows.Forms;

namespace FM
{
    partial class AddInvestment
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer? components = null;

        /// <summary>Clean up any resources being used.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // Labels
            lblName = new Label();
            lblAmount = new Label();
            lblDate = new Label();
            lblCategory = new Label();
            lblLength = new Label();
            lblNotes = new Label();

            // Inputs
            txtName = new TextBox();
            numAmount = new NumericUpDown();
            dtDate = new DateTimePicker();
            cbCategory = new ComboBox();
            cbLength = new ComboBox();
            txtNotes = new TextBox();

            // Buttons
            btnSave = new Button();
            btnViewBills = new Button();
            btnViewAllPayments = new Button();

            ((System.ComponentModel.ISupportInitialize)numAmount).BeginInit();
            SuspendLayout();

            // --- Label: Name ---
            lblName.AutoSize = true;
            lblName.Location = new Point(24, 24);
            lblName.Name = "lblName";
            lblName.Size = new Size(42, 15);
            lblName.Text = "Name:";

            // --- TextBox: Name ---
            txtName.Location = new Point(120, 21);
            txtName.Name = "txtName";
            txtName.Size = new Size(260, 23);
            txtName.TabIndex = 0;

            // --- Label: Amount ---
            lblAmount.AutoSize = true;
            lblAmount.Location = new Point(24, 64);
            lblAmount.Name = "lblAmount";
            lblAmount.Size = new Size(56, 15);
            lblAmount.Text = "Amount:";

            // --- NumericUpDown: Amount ---
            numAmount.Location = new Point(120, 61);
            numAmount.Name = "numAmount";
            numAmount.Size = new Size(120, 23);
            numAmount.DecimalPlaces = 2;
            numAmount.Maximum = 1000000000;
            numAmount.Minimum = 0;
            numAmount.ThousandsSeparator = true;
            numAmount.TabIndex = 1;

            // --- Label: Date ---
            lblDate.AutoSize = true;
            lblDate.Location = new Point(24, 104);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(34, 15);
            lblDate.Text = "Date:";

            // --- DateTimePicker: Date ---
            dtDate.Location = new Point(120, 101);
            dtDate.Name = "dtDate";
            dtDate.Size = new Size(200, 23);
            dtDate.TabIndex = 2;

            // --- Label: Category ---
            lblCategory.AutoSize = true;
            lblCategory.Location = new Point(24, 144);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new Size(58, 15);
            lblCategory.Text = "Category:";

            // --- ComboBox: Category ---
            cbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCategory.Location = new Point(120, 141);
            cbCategory.Name = "cbCategory";
            cbCategory.Size = new Size(200, 23);
            cbCategory.TabIndex = 3;
            // Add your categories here:
            // cbCategory.Items.AddRange(new object[] { "Stocks", "ETF", "Crypto", "Real Estate", "Other" });

            // --- Label: Length ---
            lblLength.AutoSize = true;
            lblLength.Location = new Point(24, 184);
            lblLength.Name = "lblLength";
            lblLength.Size = new Size(47, 15);
            lblLength.Text = "Length:";

            // --- ComboBox: Length (matches your Bill input's cbLength) ---
            cbLength.DropDownStyle = ComboBoxStyle.DropDownList;
            cbLength.Location = new Point(120, 181);
            cbLength.Name = "cbLength";
            cbLength.Size = new Size(200, 23);
            cbLength.TabIndex = 4;
            cbLength.Items.AddRange(new object[]
            {
                "One-time",
                "Weekly",
                "Biweekly",
                "Monthly",
                "Quarterly",
                "Yearly"
            });

            // --- Label: Notes ---
            lblNotes.AutoSize = true;
            lblNotes.Location = new Point(24, 224);
            lblNotes.Name = "lblNotes";
            lblNotes.Size = new Size(41, 15);
            lblNotes.Text = "Notes:";

            // --- TextBox: Notes (multiline) ---
            txtNotes.Location = new Point(120, 221);
            txtNotes.Multiline = true;
            txtNotes.Name = "txtNotes";
            txtNotes.Size = new Size(360, 80);
            txtNotes.TabIndex = 5;

            // --- Buttons ---
            btnSave.Location = new Point(120, 320);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(100, 30);
            btnSave.TabIndex = 6;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;

            btnViewBills.Location = new Point(230, 320);
            btnViewBills.Name = "btnViewBills";
            btnViewBills.Size = new Size(120, 30);
            btnViewBills.TabIndex = 7;
            btnViewBills.Text = "View Bills";
            btnViewBills.UseVisualStyleBackColor = true;
            btnViewBills.Click += btnViewBills_Click;

            btnViewAllPayments.Location = new Point(360, 320);
            btnViewAllPayments.Name = "btnViewAllPayments";
            btnViewAllPayments.Size = new Size(160, 30);
            btnViewAllPayments.TabIndex = 8;
            btnViewAllPayments.Text = "View All Payments";
            btnViewAllPayments.UseVisualStyleBackColor = true;
            btnViewAllPayments.Click += btnViewAllPayments_Click;

            // --- Form settings ---
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(520, 370);
            Controls.Add(lblName);
            Controls.Add(txtName);
            Controls.Add(lblAmount);
            Controls.Add(numAmount);
            Controls.Add(lblDate);
            Controls.Add(dtDate);
            Controls.Add(lblCategory);
            Controls.Add(cbCategory);
            Controls.Add(lblLength);
            Controls.Add(cbLength);
            Controls.Add(lblNotes);
            Controls.Add(txtNotes);
            Controls.Add(btnSave);
            Controls.Add(btnViewBills);
            Controls.Add(btnViewAllPayments);

            Name = "AddInvestment";
            Text = "Add Investment";

            ((System.ComponentModel.ISupportInitialize)numAmount).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblName;
        private Label lblAmount;
        private Label lblDate;
        private Label lblCategory;
        private Label lblLength;
        private Label lblNotes;

        private TextBox txtName;
        private NumericUpDown numAmount;
        private DateTimePicker dtDate;
        private ComboBox cbCategory;
        public ComboBox cbLength; // public if other code references it like your Bill form
        private TextBox txtNotes;

        private Button btnSave;
        private Button btnViewBills;
        private Button btnViewAllPayments;
    }
}
