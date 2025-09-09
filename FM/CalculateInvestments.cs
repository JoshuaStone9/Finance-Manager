using Npgsql;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace FM
{
    public partial class CalculateInvestments : Form
    {
        private const string ConnStr =
            "Host=localhost;Database=Finance_Manager;Username=postgres;Password=banana001;SslMode=Disable";

        // UI
        private DataGridView dgvInvestments;
        private Button btnRetrieveInvestments;
        private TextBox txtTotal;

        private Label lblJanuary; private TextBox txtJanuary;
        private Label lblFebruary; private TextBox txtFebruary;
        private Label lblMarch; private TextBox txtMarch;
        private Label lblApril; private TextBox txtApril;
        private Label lblMay; private TextBox txtMay;
        private Label lblJune; private TextBox txtJune;
        private Label lblJuly; private TextBox txtJuly;
        private Label lblAugust; private TextBox txtAugust;
        private Label lblSeptember; private TextBox txtSeptember;
        private Label lblOctober; private TextBox txtOctober;
        private Label lblNovember; private TextBox txtNovember;
        private Label lblDecember; private TextBox txtDecember;

        private Label lblInvestmentsList;

        public CalculateInvestments()
        {
            InitializeComponent();
            BuildUi();
            WireMonthClicks();
        }

        private void BuildUi()
        {
            Text = "Calculate Investments";
            ClientSize = new Size(800, 720);
            StartPosition = FormStartPosition.CenterScreen;

            btnRetrieveInvestments = new Button
            {
                Text = "Retrieve Investments",
                Location = new Point(20, 20),
                Size = new Size(180, 30)
            };
            btnRetrieveInvestments.Click += RetrieveInvestments;

            txtTotal = new TextBox
            {
                Location = new Point(220, 20),
                Width = 140,
                ReadOnly = true
            };

            // Month labels/textboxes (two rows)
            lblJanuary = new Label { Text = "January", Location = new Point(20, 60), AutoSize = true };
            txtJanuary = new TextBox { Location = new Point(100, 60), Width = 100, ReadOnly = true };
            lblFebruary = new Label { Text = "February", Location = new Point(220, 60), AutoSize = true };
            txtFebruary = new TextBox { Location = new Point(300, 60), Width = 100, ReadOnly = true };
            lblMarch = new Label { Text = "March", Location = new Point(420, 60), AutoSize = true };
            txtMarch = new TextBox { Location = new Point(500, 60), Width = 100, ReadOnly = true };
            lblApril = new Label { Text = "April", Location = new Point(620, 60), AutoSize = true };
            txtApril = new TextBox { Location = new Point(680, 60), Width = 100, ReadOnly = true };

            lblMay = new Label { Text = "May", Location = new Point(20, 100), AutoSize = true };
            txtMay = new TextBox { Location = new Point(100, 100), Width = 100, ReadOnly = true };
            lblJune = new Label { Text = "June", Location = new Point(220, 100), AutoSize = true };
            txtJune = new TextBox { Location = new Point(300, 100), Width = 100, ReadOnly = true };
            lblJuly = new Label { Text = "July", Location = new Point(420, 100), AutoSize = true };
            txtJuly = new TextBox { Location = new Point(500, 100), Width = 100, ReadOnly = true };
            lblAugust = new Label { Text = "August", Location = new Point(620, 100), AutoSize = true };
            txtAugust = new TextBox { Location = new Point(680, 100), Width = 100, ReadOnly = true };

            lblSeptember = new Label { Text = "September", Location = new Point(20, 140), AutoSize = true };
            txtSeptember = new TextBox { Location = new Point(100, 140), Width = 100, ReadOnly = true };
            lblOctober = new Label { Text = "October", Location = new Point(220, 140), AutoSize = true };
            txtOctober = new TextBox { Location = new Point(300, 140), Width = 100, ReadOnly = true };
            lblNovember = new Label { Text = "November", Location = new Point(420, 140), AutoSize = true };
            txtNovember = new TextBox { Location = new Point(500, 140), Width = 100, ReadOnly = true };
            lblDecember = new Label { Text = "December", Location = new Point(620, 140), AutoSize = true };
            txtDecember = new TextBox { Location = new Point(700, 140), Width = 100, ReadOnly = true };

            dgvInvestments = new DataGridView
            {
                AutoGenerateColumns = true,
                Location = new Point(20, 190),
                Size = new Size(760, 360),
                ReadOnly = true,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            };
            dgvInvestments.DataBindingComplete += (s, e) => UpdateMonthlyAndList(); // fill totals + list current month
            dgvInvestments.CellValueChanged += (s, e) => UpdateMonthlyAndList();
            dgvInvestments.RowsRemoved += (s, e) => UpdateMonthlyAndList();

            // Big multi-line label for listing investments of a month
            lblInvestmentsList = new Label
            {
                Location = new Point(20, 560),
                Size = new Size(760, 130),
                AutoSize = false,
                BorderStyle = BorderStyle.FixedSingle
            };

            Controls.AddRange(new Control[]
            {
                btnRetrieveInvestments, txtTotal,
                lblJanuary, txtJanuary, lblFebruary, txtFebruary, lblMarch, txtMarch, lblApril, txtApril,
                lblMay, txtMay, lblJune, txtJune, lblJuly, txtJuly, lblAugust, txtAugust,
                lblSeptember, txtSeptember, lblOctober, txtOctober, lblNovember, txtNovember, lblDecember, txtDecember,
                dgvInvestments, lblInvestmentsList
            });
        }

        private void WireMonthClicks()
        {
            txtJanuary.Click += (s, e) => UpdateMonthlyAndList(null, 1);
            txtFebruary.Click += (s, e) => UpdateMonthlyAndList(null, 2);
            txtMarch.Click += (s, e) => UpdateMonthlyAndList(null, 3);
            txtApril.Click += (s, e) => UpdateMonthlyAndList(null, 4);
            txtMay.Click += (s, e) => UpdateMonthlyAndList(null, 5);
            txtJune.Click += (s, e) => UpdateMonthlyAndList(null, 6);
            txtJuly.Click += (s, e) => UpdateMonthlyAndList(null, 7);
            txtAugust.Click += (s, e) => UpdateMonthlyAndList(null, 8);
            txtSeptember.Click += (s, e) => UpdateMonthlyAndList(null, 9);
            txtOctober.Click += (s, e) => UpdateMonthlyAndList(null, 10);
            txtNovember.Click += (s, e) => UpdateMonthlyAndList(null, 11);
            txtDecember.Click += (s, e) => UpdateMonthlyAndList(null, 12);
        }

        private void RetrieveInvestments(object? sender, EventArgs e)
        {
            using var conn = new NpgsqlConnection(ConnStr);
            conn.Open();

            const string sql = @"
                SELECT
                    investments_id,
                    name,
                    amount,
                    date::date AS date,
                    category,
                    length,
                    notes
                FROM investments
                ORDER BY investments_id DESC;";

            using var da = new NpgsqlDataAdapter(sql, conn);
            var dt = new DataTable();
            da.Fill(dt);

            dgvInvestments.DataSource = dt;

            if (dgvInvestments.Columns.Contains("amount"))
                dgvInvestments.Columns["amount"].DefaultCellStyle.Format = "N2";
            if (dgvInvestments.Columns.Contains("date"))
                dgvInvestments.Columns["date"].DefaultCellStyle.Format = "d";

            // Fill totals and list CURRENT month after loading
            UpdateMonthlyAndList(null, DateTime.Today.Month);
        }

        // Helper that fills monthly totals and (optionally) lists a specific month
        private void UpdateMonthlyAndList(int? year = null, int? monthToList = null)
        {
            UpdateMonthlyTotals(year, out var totals);
            WriteTotalsToTextboxes(totals);
            UpdateLabelList(monthToList ?? DateTime.Today.Month, year ?? DateTime.Today.Year);
        }

        // 1) Compute month totals for a given year
        private void UpdateMonthlyTotals(int? year, out decimal[] totals)
        {
            int targetYear = year ?? DateTime.Today.Year;
            totals = new decimal[13]; // 1..12

            if (dgvInvestments.DataSource is DataTable dt)
            {
                foreach (DataRow r in dt.Rows)
                {
                    if (r.IsNull("date") || r.IsNull("amount")) continue;
                    var d = (DateTime)r["date"];
                    if (d.Year != targetYear) continue;

                    decimal amt = r["amount"] is decimal dec ? dec : Convert.ToDecimal(r["amount"]);
                    totals[d.Month] += amt;
                }
            }
            else
            {
                foreach (DataGridViewRow row in dgvInvestments.Rows)
                {
                    if (row.IsNewRow) continue;

                    if (DateTime.TryParse(row.Cells["date"].Value?.ToString(), out var d) &&
                        decimal.TryParse(row.Cells["amount"].Value?.ToString(), out var amt) &&
                        d.Year == targetYear)
                    {
                        totals[d.Month] += amt;
                    }
                }
            }
            // update grand total textbox
            txtTotal.Text = totals.Sum().ToString("N2");
        }

        // 2) Write totals into the 12 month textboxes
        private void WriteTotalsToTextboxes(decimal[] totals)
        {
            txtJanuary.Text = totals[1].ToString("N2");
            txtFebruary.Text = totals[2].ToString("N2");
            txtMarch.Text = totals[3].ToString("N2");
            txtApril.Text = totals[4].ToString("N2");
            txtMay.Text = totals[5].ToString("N2");
            txtJune.Text = totals[6].ToString("N2");
            txtJuly.Text = totals[7].ToString("N2");
            txtAugust.Text = totals[8].ToString("N2");
            txtSeptember.Text = totals[9].ToString("N2");
            txtOctober.Text = totals[10].ToString("N2");
            txtNovember.Text = totals[11].ToString("N2");
            txtDecember.Text = totals[12].ToString("N2");
        }

        // 3) Build the multi-line list for a month and show in label
        private void UpdateLabelList(int month, int year)
        {
            var lines = new System.Collections.Generic.List<string>();

            if (dgvInvestments.DataSource is DataTable dt)
            {
                foreach (DataRow r in dt.Rows)
                {
                    if (r.IsNull("date") || r.IsNull("amount")) continue;

                    var d = (DateTime)r["date"];
                    if (d.Month == month && d.Year == year)
                    {
                        string name = dt.Columns.Contains("name") && !r.IsNull("name") ? r["name"].ToString()! : "";
                        decimal amt = r["amount"] is decimal dec ? dec : Convert.ToDecimal(r["amount"]);
                        lines.Add($"{name} - {amt:N2} on {d:dd-MMM}");
                    }
                }
            }
            else
            {
                foreach (DataGridViewRow row in dgvInvestments.Rows)
                {
                    if (row.IsNewRow) continue;

                    if (DateTime.TryParse(row.Cells["date"].Value?.ToString(), out var d) &&
                        decimal.TryParse(row.Cells["amount"].Value?.ToString(), out var amt) &&
                        d.Month == month && d.Year == year)
                    {
                        string name = dgvInvestments.Columns.Contains("name")
                                      ? row.Cells["name"].Value?.ToString() ?? ""
                                      : "";
                        lines.Add($"{name} - {amt:N2} on {d:dd-MMM}");
                    }
                }
            }

            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            lblInvestmentsList.Text = lines.Count == 0
                ? $"No investments for {monthName} {year}"
                : $"{monthName} {year}:\n" + string.Join(Environment.NewLine, lines);
        }
    }
}
