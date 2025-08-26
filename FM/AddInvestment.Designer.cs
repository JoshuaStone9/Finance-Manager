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
            lblOtherInvestment = new Label();

            // Inputs
            txtName = new TextBox();
            txtAmount = new TextBox();
            dtDate = new DateTimePicker();
            cbCategory = new ComboBox();
            cbLength = new ComboBox();
            txtNotes = new TextBox();
            txtOtherInvestment = new TextBox();

            // Buttons
            btnSave = new Button();
            btnViewBills = new Button();
            btnViewAllPayments = new Button();

     
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
            txtAmount.AutoSize = true;
            txtAmount.Location = new Point(44, 64);
            txtAmount.Name = "txtAmount";
            txtAmount.Size = new Size(56, 15);
            txtAmount.TabIndex = 1;

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
            cbCategory.Items.AddRange(new object[] { "Stocks", "ETF", "Crypto", "Real Estate", "Other" });
            cbCategory.SelectedIndexChanged += cbCategory_SelectedIndexChanged;

            // --- Label: Other Investment ---
            lblOtherInvestment.AutoSize = true;
            lblOtherInvestment.Location = new Point(24, 184);
            lblOtherInvestment.Name = "lblOtherInvestment";
            lblOtherInvestment.Size = new Size(109, 15);
            lblOtherInvestment.Text = "Other Investment:";
            lblOtherInvestment.Visible = false;

            // --- TextBox: Other Investment ---
            txtOtherInvestment.Location = new Point(140, 181);
            txtOtherInvestment.Name = "txtOtherInvestment";
            txtOtherInvestment.Size = new Size(260, 23);
            txtOtherInvestment.TabIndex = 4;
            txtOtherInvestment.Visible = false;

            if(cbCategory.SelectedItem?.ToString() == "Other")
            {
                lblOtherInvestment.Visible = true;
                txtOtherInvestment.Visible = true;
            }


            // --- Label: Length ---
            lblLength.AutoSize = true;
            lblLength.Location = new Point(24, 224);
            lblLength.Name = "lblLength";
            lblLength.Size = new Size(47, 15);
            lblLength.Text = "Length:";

            // --- ComboBox: Length (matches your Bill input's cbLength) ---
            cbLength.DropDownStyle = ComboBoxStyle.DropDownList;
            cbLength.Location = new Point(120, 221);
            cbLength.Name = "cbLength";
            cbLength.Size = new Size(200, 23);
            cbLength.TabIndex = 5;
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
            lblNotes.Location = new Point(24, 264);
            lblNotes.Name = "lblNotes";
            lblNotes.Size = new Size(41, 15);
            lblNotes.Text = "Notes:";

            // --- TextBox: Notes (multiline) ---
            txtNotes.Location = new Point(120, 261);
            txtNotes.Multiline = true;
            txtNotes.Name = "txtNotes";
            txtNotes.Size = new Size(360, 80);
            txtNotes.TabIndex = 6;

            // --- Buttons ---
            btnSave.Location = new Point(120, 360);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(100, 30);
            btnSave.TabIndex = 7;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;

            btnViewBills.Location = new Point(230, 360);
            btnViewBills.Name = "btnViewBills";
            btnViewBills.Size = new Size(120, 30);
            btnViewBills.TabIndex = 8;
            btnViewBills.Text = "View Bills";
            btnViewBills.UseVisualStyleBackColor = true;
            btnViewBills.Click += btnViewBills_Click;

            btnViewAllPayments.Location = new Point(360, 360);
            btnViewAllPayments.Name = "btnViewAllPayments";
            btnViewAllPayments.Size = new Size(160, 30);
            btnViewAllPayments.TabIndex = 9;
            btnViewAllPayments.Text = "View All Payments";
            btnViewAllPayments.UseVisualStyleBackColor = true;
            btnViewAllPayments.Click += btnViewAllPayments_Click;

            // --- Form settings ---
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(520, 410);
            Controls.Add(lblName);
            Controls.Add(txtName);
            Controls.Add(lblAmount);
            Controls.Add(txtAmount);
            Controls.Add(lblDate);
            Controls.Add(dtDate);
            Controls.Add(lblCategory);
            Controls.Add(cbCategory);
            Controls.Add(lblOtherInvestment);
            Controls.Add(txtOtherInvestment);
            Controls.Add(lblLength);
            Controls.Add(cbLength);
            Controls.Add(lblNotes);
            Controls.Add(txtNotes);
            Controls.Add(btnSave);
            Controls.Add(btnViewBills);
            Controls.Add(btnViewAllPayments);

            Name = "AddInvestment";
            Text = "Add Investment";

            ResumeLayout(false);
            PerformLayout();
        }

        private void cbCategory_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (cbCategory.SelectedItem.ToString() == "Other")
            {
                lblOtherInvestment.Visible = true;
                txtOtherInvestment.Visible = true;
            }
            else
            {
                lblOtherInvestment.Visible = false;
                txtOtherInvestment.Visible = false;
            }
        }

        #endregion

        private Label lblName;
        private Label lblAmount;
        private Label lblDate;
        private Label lblCategory;
        private Label lblLength;
        private Label lblNotes;
        private Label lblOtherInvestment;

        private TextBox txtName;
        private TextBox txtAmount;
        private DateTimePicker dtDate;
        private ComboBox cbCategory;
        public ComboBox cbLength; // public if other code references it like your Bill form
        private TextBox txtNotes;
        private TextBox txtOtherInvestment;

        private Button btnSave;
        private Button btnViewBills;
        private Button btnViewAllPayments;
    }
}
