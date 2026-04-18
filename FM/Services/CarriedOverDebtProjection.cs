using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace FM
{
    public partial class CarriedOverDebtProjection : Form
    {
        private PictureBox logo;
        private Label title;
        private Label lblSubtitle;

        private Button btnCalculate;
        private Button btnClose;

        private Label lblStartingDebt;
        private TextBox txtStartingDebt;

        private Label lblMonths;
        private NumericUpDown numMonths;

        private Label lblMonthlyPayment;
        private TextBox txtMonthlyPayment;

        private Label lblProjectedDebt;
        private TextBox txtProjectedDebt;

        private DataGridView gridProjection = new() { ReadOnly = true };

        public CarriedOverDebtProjection()
        {
            InitializeComponent();

            AutoScroll = true;
            AutoScrollMargin = new Size(0, 20);
            Text = "Carried Over Debt Projection";
            ClientSize = new Size(1200, 900);
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;
            Font = new Font("Montserrat", 10, FontStyle.Regular);
            Paint += CarriedOverDebtProjection_Paint;

            BuildUi();
        }

        private void BuildUi()
        {
            Controls.Clear();

            logo = new PictureBox
            {
                Image = Image.FromFile("images/FM_Logo_Main_Menu.png"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(120, 120),
                Location = new Point((ClientSize.Width - 120) / 2, 10),
                BackColor = Color.Transparent
            };
            Controls.Add(logo);

            title = new Label
            {
                Text = "Carried Over Debt Projection",
                Font = new Font("Montserrat", 14F, FontStyle.Bold),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point((ClientSize.Width - 260) / 2, 135)
            };
            Controls.Add(title);

            lblSubtitle = new Label
            {
                Text = "Project shortfall over future months",
                Font = new Font("Montserrat", 9F, FontStyle.Regular),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point((ClientSize.Width - 220) / 2, 165)
            };
            Controls.Add(lblSubtitle);

            btnCalculate = MakePrimaryButton("Calculate", new Point(20, 210), new Size(140, 40), (s, e) => CalculateProjection());
            btnClose = MakeSecondaryButton("Close", new Point(170, 210), new Size(140, 40), (s, e) => Close());

            Controls.Add(btnCalculate);
            Controls.Add(btnClose);

            var inputPanel = new Panel
            {
                Location = new Point(20, 270),
                Size = new Size(ClientSize.Width - 40, 140),
                BackColor = Color.Transparent
            };
            Controls.Add(inputPanel);

            lblStartingDebt = MakeLabel("Starting Debt:", new Point(10, 20));
            txtStartingDebt = MakeReadWriteTextBox(new Point(140, 16), 140);

            lblMonths = MakeLabel("Months:", new Point(320, 20));
            numMonths = new NumericUpDown
            {
                Location = new Point(390, 16),
                Width = 100,
                Minimum = 1,
                Maximum = 60,
                Value = 12
            };

            lblMonthlyPayment = MakeLabel("Monthly Payment:", new Point(530, 20));
            txtMonthlyPayment = MakeReadWriteTextBox(new Point(670, 16), 140);

            lblProjectedDebt = MakeLabel("Projected Debt:", new Point(850, 20));
            txtProjectedDebt = new TextBox
            {
                Location = new Point(980, 16),
                Width = 140,
                ReadOnly = true,
                TextAlign = HorizontalAlignment.Right
            };

            inputPanel.Controls.Add(lblStartingDebt);
            inputPanel.Controls.Add(txtStartingDebt);
            inputPanel.Controls.Add(lblMonths);
            inputPanel.Controls.Add(numMonths);
            inputPanel.Controls.Add(lblMonthlyPayment);
            inputPanel.Controls.Add(txtMonthlyPayment);
            inputPanel.Controls.Add(lblProjectedDebt);
            inputPanel.Controls.Add(txtProjectedDebt);

            SetupGrid(gridProjection, "Projection Breakdown", new Point(20, 440), ClientSize.Width - 40, 360);
            Controls.Add(gridProjection);
        }

        private void CalculateProjection()
        {
            if (!decimal.TryParse(txtStartingDebt.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-GB"), out var startingDebt))
            {
                MessageBox.Show("Enter a valid starting debt amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtMonthlyPayment.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-GB"), out var monthlyPayment))
            {
                MessageBox.Show("Enter a valid monthly payment amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int months = (int)numMonths.Value;
            decimal runningDebt = startingDebt;

            var dt = new System.Data.DataTable();
            dt.Columns.Add("Month");
            dt.Columns.Add("Starting Debt", typeof(decimal));
            dt.Columns.Add("Payment", typeof(decimal));
            dt.Columns.Add("Ending Debt", typeof(decimal));

            for (int i = 1; i <= months; i++)
            {
                decimal monthStart = runningDebt;
                runningDebt = Math.Max(0, runningDebt - monthlyPayment);

                dt.Rows.Add(
                    $"Month {i}",
                    monthStart,
                    monthlyPayment,
                    runningDebt);
            }

            gridProjection.DataSource = dt;
            FormatGrid(gridProjection);

            txtProjectedDebt.Text = runningDebt.ToString("C", CultureInfo.GetCultureInfo("en-GB"));
        }

        private Label MakeLabel(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Location = location,
                AutoSize = true,
                BackColor = Color.Transparent
            };
        }

        private TextBox MakeReadWriteTextBox(Point location, int width)
        {
            return new TextBox
            {
                Location = location,
                Width = width,
                TextAlign = HorizontalAlignment.Right
            };
        }

        private Button MakePrimaryButton(string text, Point location, Size size, EventHandler onClick)
        {
            var b = new Button
            {
                Text = text,
                Location = location,
                Size = size,
                BackColor = Color.FromArgb(255, 120, 120),
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderColor = Color.Black;
            b.FlatAppearance.BorderSize = 2;
            b.Click += onClick;
            return b;
        }

        private Button MakeSecondaryButton(string text, Point location, Size size, EventHandler onClick)
        {
            var b = new Button
            {
                Text = text,
                Location = location,
                Size = size,
                BackColor = Color.FromArgb(255, 150, 150),
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderColor = Color.Black;
            b.FlatAppearance.BorderSize = 2;
            b.Click += onClick;
            return b;
        }

        private void SetupGrid(DataGridView grid, string caption, Point location, int width, int height)
        {
            var lbl = new Label
            {
                Text = caption,
                Location = new Point(location.X, location.Y - 28),
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            Controls.Add(lbl);

            grid.Location = location;
            grid.Size = new Size(width, height);
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowHeadersVisible = false;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.EnableHeadersVisualStyles = false;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 150, 150);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font(Font, FontStyle.Bold);
        }

        private void FormatGrid(DataGridView grid)
        {
            if (grid.Columns.Contains("Starting Debt"))
            {
                grid.Columns["Starting Debt"].DefaultCellStyle.Format = "C";
                grid.Columns["Starting Debt"].DefaultCellStyle.FormatProvider = new CultureInfo("en-GB");
            }

            if (grid.Columns.Contains("Payment"))
            {
                grid.Columns["Payment"].DefaultCellStyle.Format = "C";
                grid.Columns["Payment"].DefaultCellStyle.FormatProvider = new CultureInfo("en-GB");
            }

            if (grid.Columns.Contains("Ending Debt"))
            {
                grid.Columns["Ending Debt"].DefaultCellStyle.Format = "C";
                grid.Columns["Ending Debt"].DefaultCellStyle.FormatProvider = new CultureInfo("en-GB");
            }
        }

        private void CarriedOverDebtProjection_Paint(object? sender, PaintEventArgs e)
        {
            using var brush = new LinearGradientBrush(ClientRectangle, Color.LightCoral, Color.White, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }
    }
}