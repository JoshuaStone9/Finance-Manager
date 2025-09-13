using Npgsql;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace FM
{
    public partial class AllPayments : Form
    {
        private readonly string connectionString =
            "Host=localhost;Database=Finance_Manager;Username=postgres;Password=banana001;SslMode=Disable";

        private readonly DataGridView gridBills = new() { ReadOnly = true };
        private readonly DataGridView gridExpenses = new() { ReadOnly = true };
        private readonly DataGridView gridInvestments = new() { ReadOnly = true };
        private readonly DataGridView gridSavings = new() { ReadOnly = true };

        private TextBox txtGrandTotal;
        private Label lblGrandTotal;
        private Panel bottomPanel;
        private PictureBox logo;
        private Label title;

        private Button btnReload;

        public AllPayments()
        {
            InitializeComponent();

            // ---- Form styling (consistent with app) ----
            Text = "All Payments";
            ClientSize = new Size(1200, 1060);
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;
            Font = new Font("Montserrat", 10, FontStyle.Regular);
            Paint += AllPayments_Paint;

            BuildUi();
            Load += AllPayments_Load;
        }

        private void BuildUi()
        {
            Controls.Clear();

            // Optional top-center logo
            logo = new PictureBox
            {
                Image = Image.FromFile("images/FM_Logo_Main_Menu.png"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(120, 120),
                Location = new Point((ClientSize.Width - 120) / 2, 0),
                BackColor = Color.Transparent
            };
            Controls.Add(logo);

            // Title
            title = new Label
            {
                Text = "All Payments",
                Font = new Font("Montserrat", 14F, FontStyle.Bold),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point((ClientSize.Width - 160) / 2, 120)
            };
            Controls.Add(title);

            // Reload button
            btnReload = MakePrimaryButton("Reload", new Point(16, 150), new Size(140, 40), (s, e) => ReloadAll());
            Controls.Add(btnReload);

            // Grids
            int left = 16;
            int width = ClientSize.Width - 32;

            SetupGrid(gridBills, "Bills", new Point(left, 200), width, 200);
            SetupGrid(gridExpenses, "Extra Expenses", new Point(left, 420), width, 200);
            SetupGrid(gridInvestments, "Investments", new Point(left, 640), width, 160);
            SetupGrid(gridSavings, "Savings", new Point(left, 800), width, 200); // Hidden for now

            Controls.AddRange(new Control[] { gridBills, gridExpenses, gridInvestments, gridSavings });

            //test 
            // Bottom Total panel
            bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                Padding = new Padding(12),
                BackColor = Color.Transparent
            };

            var totalLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                WrapContents = false,
                Padding = new Padding(0, 10, 0, 0),
                Margin = new Padding(0)
            };

            txtGrandTotal = new TextBox
            {
                ReadOnly = true,
                Width = 220,
                TextAlign = HorizontalAlignment.Right
            };

            lblGrandTotal = new Label
            {
                Text = "Grand Total:",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 4, 0, 0),
                Margin = new Padding(0, 0, 8, 0),
                BackColor = Color.Transparent
            };

            totalLayout.Controls.Add(txtGrandTotal);
            totalLayout.Controls.Add(lblGrandTotal);
            bottomPanel.Controls.Add(totalLayout);
            Controls.Add(bottomPanel);
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

        private void SetupGrid(DataGridView grid, string caption, Point location, int width, int height)
        {
            // Section label
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
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowHeadersVisible = false;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.EnableHeadersVisualStyles = false;

            // Header style to match theme
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 150, 150);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font(Font, FontStyle.Bold);

            // When data changes, reformat and update total
            grid.DataBindingComplete += (s, e) => { FormatGrid(grid); UpdateTotal(); };
            grid.CellValueChanged += (s, e) => UpdateTotal();
            grid.RowsRemoved += (s, e) => UpdateTotal();
        }

        private void AllPayments_Load(object? sender, EventArgs e)
        {
            ReloadAll();
        }

        private void ReloadAll()
        {
            BillsDataGrid();
            ExtraExpenseDataGrid();
            InvestmentsDataGrid();
            SavingsDataGrid();
            UpdateTotal();
        }

        // Gradient background
        private void AllPayments_Paint(object? sender, PaintEventArgs e)
        {
            using var brush = new LinearGradientBrush(ClientRectangle, Color.LightCoral, Color.White, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        // ---- Data loading (unchanged logic, just wrapped) ----
        private void BillsDataGrid()
        {
            using var con = new NpgsqlConnection(connectionString);
            try
            {
                con.Open();
                string query = "SELECT billid, name, amount, date, type, length, description FROM bills ORDER BY date DESC";
                var da = new NpgsqlDataAdapter(query, con);
                var dt = new DataTable();
                da.Fill(dt);
                gridBills.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bills error: " + ex.Message);
            }
        }

        private void InvestmentsDataGrid()
        {
            using var con = new NpgsqlConnection(connectionString);
            try
            {
                con.Open();
                string query = "SELECT investments_id, name, amount, date, category, length, notes FROM investments ORDER BY date DESC";
                var da = new NpgsqlDataAdapter(query, con);
                var dt = new DataTable();
                da.Fill(dt);
                gridInvestments.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Investments error: " + ex.Message);
            }
        }

        private void ExtraExpenseDataGrid()
        {
            using var con = new NpgsqlConnection(connectionString);
            try
            {
                con.Open();
                string query = "SELECT extra_expense_id, name, amount, duedate AS date, category, type, length, description FROM extra_expenses ORDER BY duedate DESC";
                var da = new NpgsqlDataAdapter(query, con);
                var dt = new DataTable();
                da.Fill(dt);
                gridExpenses.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Extra expenses error: " + ex.Message);
            }
        }

        private void SavingsDataGrid()
        {
            using var con = new NpgsqlConnection(connectionString);
            try
            {
                con.Open();
                string query = "SELECT savings_id, name, amount, length, date, notes FROM savings ORDER BY date DESC";
                var da = new NpgsqlDataAdapter(query, con);
                var dt = new DataTable();
                da.Fill(dt);
                gridSavings.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Savings error: " + ex.Message);
            }
        }

        // ---- Formatting + totals ----
        private void FormatGrid(DataGridView grid)
        {
            // Amount as currency (GBP), Date short
            if (grid.Columns.Contains("amount"))
            {
                grid.Columns["amount"].DefaultCellStyle.Format = "C";
                grid.Columns["amount"].DefaultCellStyle.FormatProvider = new CultureInfo("en-GB");
                grid.Columns["amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (grid.Columns.Contains("date"))
            {
                grid.Columns["date"].DefaultCellStyle.Format = "d";
                grid.Columns["date"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void UpdateTotal()
        {
            decimal total = 0m;

            total += SumAmountColumn(gridBills);
            total += SumAmountColumn(gridExpenses);
            total += SumAmountColumn(gridInvestments);

            txtGrandTotal.Text = total.ToString("C", CultureInfo.GetCultureInfo("en-GB"));
        }

        private decimal SumAmountColumn(DataGridView grid)
        {
            if (grid.DataSource is not DataTable dt || !dt.Columns.Contains("amount"))
                return 0m;

            decimal sum = 0m;
            foreach (DataRow r in dt.Rows)
            {
                if (r.IsNull("amount")) continue;
                // amount might be decimal or string
                if (r["amount"] is decimal dec) sum += dec;
                else if (decimal.TryParse(Convert.ToString(r["amount"]), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                    sum += val;
                else if (decimal.TryParse(Convert.ToString(r["amount"]), NumberStyles.Any, CultureInfo.CurrentCulture, out var val2))
                    sum += val2;
            }
            return sum;
        }
    }
}
