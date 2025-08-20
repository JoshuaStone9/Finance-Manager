using System.Drawing;
using System.Windows.Forms;

namespace FM
{
    partial class BillsRecord
    {
        private System.ComponentModel.IContainer? components = null;
        private DataGridView gridBills;
        private Button btnDeleteBills;
        private Button btnCloseBills;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            gridBills = new DataGridView();
            btnDeleteBills = new Button();
            btnCloseBills = new Button();

            ((System.ComponentModel.ISupportInitialize)gridBills).BeginInit();
            SuspendLayout();

            // gridBills
            gridBills.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridBills.Location = new Point(12, 12);
            gridBills.Name = "gridBills";
            gridBills.Size = new Size(760, 360);
            gridBills.TabIndex = 0;

            // btnDeleteBills
            btnDeleteBills.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnDeleteBills.Location = new Point(12, 385);
            btnDeleteBills.Name = "btnDeleteBills";
            btnDeleteBills.Size = new Size(160, 27);
            btnDeleteBills.TabIndex = 1;
            btnDeleteBills.Text = "Delete selected bills";
            btnDeleteBills.UseVisualStyleBackColor = true;

            // btnCloseBills
            btnCloseBills.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCloseBills.Location = new Point(692, 385);
            btnCloseBills.Name = "btnCloseBills";
            btnCloseBills.Size = new Size(80, 27);
            btnCloseBills.TabIndex = 2;
            btnCloseBills.Text = "Close Bills";
            btnCloseBills.UseVisualStyleBackColor = true;

            // BillsRecord form
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 421);
            Controls.Add(gridBills);
            Controls.Add(btnDeleteBills);
            Controls.Add(btnCloseBills);
            Name = "BillsRecord";
            Text = "Bills";

            ((System.ComponentModel.ISupportInitialize)gridBills).EndInit();
            ResumeLayout(false);
        }
    }
}
