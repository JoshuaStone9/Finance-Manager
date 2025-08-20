using System.Drawing;
using System.Windows.Forms;

namespace FM
{
    partial class InvestmentsRecord
    {
        private System.ComponentModel.IContainer? components = null;
        private DataGridView gridInvestments;
        private Button btnDeleteInvestments;
        private Button btnCloseInvestments;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            gridInvestments = new DataGridView();
            btnDeleteInvestments = new Button();
            btnCloseInvestments = new Button();

            ((System.ComponentModel.ISupportInitialize)gridInvestments).BeginInit();
            SuspendLayout();

            // gridInvestments
            gridInvestments.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridInvestments.Location = new Point(12, 12);
            gridInvestments.Name = "gridInvestments";
            gridInvestments.Size = new Size(760, 360);
            gridInvestments.TabIndex = 0;

            // btnDeleteInvestments
            btnDeleteInvestments.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnDeleteInvestments.Location = new Point(12, 385);
            btnDeleteInvestments.Name = "btnDeleteInvestments";
            btnDeleteInvestments.Size = new Size(200, 27);
            btnDeleteInvestments.TabIndex = 1;
            btnDeleteInvestments.Text = "Delete selected investments";
            btnDeleteInvestments.UseVisualStyleBackColor = true;

            // btnCloseInvestments
            btnCloseInvestments.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCloseInvestments.Location = new Point(692, 385);
            btnCloseInvestments.Name = "btnCloseInvestments";
            btnCloseInvestments.Size = new Size(80, 27);
            btnCloseInvestments.TabIndex = 2;
            btnCloseInvestments.Text = "Close";
            btnCloseInvestments.UseVisualStyleBackColor = true;

            // InvestmentsRecord form
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 421);
            Controls.Add(gridInvestments);
            Controls.Add(btnDeleteInvestments);
            Controls.Add(btnCloseInvestments);
            Name = "InvestmentsRecord";
            Text = "Investments";

            ((System.ComponentModel.ISupportInitialize)gridInvestments).EndInit();
            ResumeLayout(false);
        }
    }
}
