using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace FM
{
    public partial class AllPayments : Form
    {
        private static string BuildConnStr()
        {
            var pwd = Environment.GetEnvironmentVariable("DB_PASSWORD", EnvironmentVariableTarget.User);
            if (string.IsNullOrWhiteSpace(pwd))
                throw new InvalidOperationException(
                    "DB_PASSWORD environment variable not set for the current user.\n" +
                    "Set it with: setx DB_PASSWORD \"YourPassword\" and restart Visual Studio/your app.");

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = "STONEY,1433",
                InitialCatalog = "Finance_Manager",
                UserID = "josh",
                Password = pwd,
                Encrypt = true,
                TrustServerCertificate = true
            };

            return builder.ConnectionString;
        }

        private readonly DataGridView gridBills = new() { ReadOnly = true };
        private readonly DataGridView gridExpenses = new() { ReadOnly = true };
        private readonly DataGridView gridInvestments = new() { ReadOnly = true };
        private readonly DataGridView gridSavings = new() { ReadOnly = true };

        private TextBox txtGrandTotal;
        private Label lblGrandTotal;
        private TextBox txtEmergencyFund;
        private Label lblEmergencyFund;
        private Button editEmergencyFund;
        private TextBox txtMonthlyAllowance;
        private Label lblMonthlyAllowance;
        private Button editMonthlyAllowance;
        private Panel bottomPanel;
        private PictureBox logo;
        private Label title;

        private Button btnReload;
        private Button btnEditSelected;
        private Button btnDeleteSelected;
        private Button btnSaveSelected;
        private Button btnfilterByDate;

        public AllPayments()
        {
            InitializeComponent();

            this.AutoScroll = true;
            this.AutoScrollMargin = new Size(0, 20);
            Text = "All Payments";
            ClientSize = new Size(1200, 1260);
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Montserrat", 10, FontStyle.Regular);
            Paint += AllPayments_Paint;

            BuildUi();
            Load += AllPayments_Load;
        }

        private void BuildUi()
        {
            Controls.Clear();

            logo = new PictureBox
            {
                Image = System.Drawing.Image.FromFile("images/FM_Logo_Main_Menu.png"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(120, 120),
                Location = new Point((ClientSize.Width - 120) / 2, 0),
                BackColor = Color.Transparent
            };
            Controls.Add(logo);


            title = new Label
            {
                Text = "All Payments",
                Font = new System.Drawing.Font("Montserrat", 14F, FontStyle.Bold),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point((ClientSize.Width - 160) / 2, 120)
            };
            Controls.Add(title);

            btnReload = MakePrimaryButton("Reload", new Point(16, 150), new Size(140, 40), (s, e) => ReloadAll());
            Controls.Add(btnReload);


            btnEditSelected = MakeSecondaryButton("Edit Selected", new Point(168, 150), new Size(140, 40), (s, e) => EditSelected());
            btnDeleteSelected = MakeSecondaryButton("Delete Selected", new Point(320, 150), new Size(140, 40), (s, e) => DeleteSelected());
            btnfilterByDate = MakeSecondaryButton("Filter by Date", new Point(472, 150), new Size(140, 40), FilterByDate_click);
            Controls.Add(btnEditSelected);
            Controls.Add(btnDeleteSelected);
            Controls.Add(btnfilterByDate);


            int left = 16;
            int width = ClientSize.Width - 32;

            SetupGrid(gridBills, "Bills", new Point(left, 200), width, 200);
            SetupGrid(gridExpenses, "Extra Expenses", new Point(left, 420), width, 200);
            SetupGrid(gridInvestments, "Investments", new Point(left, 640), width, 160);
            SetupGrid(gridSavings, "Savings", new Point(left, 800), width, 200);

            Controls.AddRange(new Control[] { gridBills, gridExpenses, gridInvestments, gridSavings });


            bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 200,
                Padding = new Padding(12, 12, 12, 20),

                BackColor = Color.Transparent
            };

            var totalLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                WrapContents = false,
                Padding = new Padding(0, 12, 0, 0),
                Margin = new Padding(0)
            };

            txtGrandTotal = new TextBox
            {
                ReadOnly = true,
                Width = 150,
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

            lblEmergencyFund = new Label
            {
                Text = "Emergency Fund:",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 4, 0, 0),
                Margin = new Padding(0, 0, 16, 0),
                BackColor = Color.Transparent
            };

            txtEmergencyFund = new TextBox
            {
                ReadOnly = true,
                Width = 150,
                TextAlign = HorizontalAlignment.Right
            };

            editEmergencyFund = new Button
            {
                Text = "Edit",
                AutoSize = true,
                Padding = new Padding(12, 6, 12, 6),
                Margin = new Padding(0, 0, 16, 0),
                BackColor = Color.FromArgb(255, 150, 150),
                FlatStyle = FlatStyle.Flat,

            };

            lblMonthlyAllowance = new Label
            {
                Text = "Monthly Allowance:",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 4, 0, 0),
                Margin = new Padding(0, 0, 16, 0),
                BackColor = Color.Transparent
            };
            txtMonthlyAllowance = new TextBox
            {
                ReadOnly = true,
                Width = 150,
                TextAlign = HorizontalAlignment.Right
            };

            editMonthlyAllowance = new Button
            {
                Text = "Edit",
                AutoSize = true,
                Padding = new Padding(12, 6, 12, 6),
                Margin = new Padding(0, 0, 16, 0),
                BackColor = Color.FromArgb(255, 150, 150),
                FlatStyle = FlatStyle.Flat,

            };


            totalLayout.Controls.Add(txtGrandTotal);
            totalLayout.Controls.Add(lblGrandTotal);
            totalLayout.Controls.Add(txtEmergencyFund);
            totalLayout.Controls.Add(lblEmergencyFund);
            totalLayout.Controls.Add(editEmergencyFund);
            totalLayout.Controls.Add(txtMonthlyAllowance);
            totalLayout.Controls.Add(lblMonthlyAllowance);
            totalLayout.Controls.Add(editMonthlyAllowance);
            editEmergencyFund.Click += EditEmergencyFund_Click;
            editMonthlyAllowance.Click += EditMonthlyAllowance_click;
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

        private void EmergencyFundData()
        {
            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();
                string query = "SELECT amount FROM dbo.emergency_fund";
                using var cmd = new SqlCommand(query, con);
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    decimal amount = Convert.ToDecimal(result);
                    txtEmergencyFund.Text = amount.ToString("C", CultureInfo.GetCultureInfo("en-GB"));
                }
                else
                {
                    txtEmergencyFund.Text = (0m).ToString("C", CultureInfo.GetCultureInfo("en-GB"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Emergency fund error: " + ex.Message);
            }
        }
        private void EditEmergencyFund_Click(object? sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
     "Enter new emergency fund amount:",
     "Edit Emergency Fund",
     txtEmergencyFund.Text);

            if (!decimal.TryParse(input, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-GB"), out var newAmount))
            {
                MessageBox.Show("Invalid amount entered.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();

                using var cmd = con.CreateCommand();
                cmd.CommandText = @"
IF EXISTS (SELECT 1 FROM dbo.emergency_fund)
    UPDATE dbo.emergency_fund SET amount = @amount;
ELSE
    INSERT INTO dbo.emergency_fund (amount) VALUES (@amount);";

                var p = cmd.Parameters.Add("@amount", SqlDbType.Decimal);
                txtEmergencyFund.Text = newAmount.ToString("C", CultureInfo.GetCultureInfo("en-GB"));
                p.Precision = 12;
                p.Scale = 2;
                p.Value = newAmount;

                cmd.ExecuteNonQuery();

                MessageBox.Show("Emergency fund updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update emergency fund: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void EditMonthlyAllowance_click(object? sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
     "Enter new monthly allowance amount:",
     "Edit Monthly Allowance",
     txtMonthlyAllowance.Text);

            if (!decimal.TryParse(input, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-GB"), out var newAmount))
            {
                MessageBox.Show("Invalid amount entered.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();

                using var cmd = con.CreateCommand();
                cmd.CommandText = @"
IF EXISTS (SELECT 1 FROM dbo.monthly_allowance)
    UPDATE dbo.monthly_allowance SET amount = @amount;
ELSE
    INSERT INTO dbo.monthly_allowance (amount) VALUES (@amount);";

                var p = cmd.Parameters.Add("@amount", SqlDbType.Decimal);
                txtEmergencyFund.Text = newAmount.ToString("C", CultureInfo.GetCultureInfo("en-GB"));
                p.Precision = 12;
                p.Scale = 2;
                p.Value = newAmount;

                cmd.ExecuteNonQuery();

                MessageBox.Show("Monthly Allowance updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update Monthly Allowance: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditMonthlyAllowance()
        {
            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();
                string query = "SELECT amount FROM dbo.monthly_allowance";
                using var cmd = new SqlCommand(query, con);
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    decimal amount = Convert.ToDecimal(result);
                    txtMonthlyAllowance.Text = amount.ToString("C", CultureInfo.GetCultureInfo("en-GB"));
                }
                else
                {
                    txtMonthlyAllowance.Text = (0m).ToString("C", CultureInfo.GetCultureInfo("en-GB"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Monthly Allowance error: " + ex.Message);
            }
        }

        private void FilterByDate_click(object? sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
        "Enter month to filter (1-12):",
        "Filter by Month",
        DateTime.Today.Month.ToString());

            if (!int.TryParse(input, out int month) || month < 1 || month > 12)
            {
                MessageBox.Show("Please enter a valid month number (1-12).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string yearInput = Microsoft.VisualBasic.Interaction.InputBox(
        "Enter year:",
        "Filter by Year",
        DateTime.Today.Year.ToString());

            if (!int.TryParse(yearInput, out int year) || year < 2000 || year > 2100)
            {
                MessageBox.Show("Please enter a valid year.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FilterGridByMonth(month, year);
        }

        private void FilterGridByMonth(int month, int year)
        {
            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();

                // Filter Bills
                string billsQuery = @"SELECT billid, name, amount, [date], type, length, description 
                             FROM dbo.bills 
                             WHERE MONTH([date]) = @month AND YEAR([date]) = @year 
                             ORDER BY [date] DESC";
                LoadFilteredData(gridBills, billsQuery, con, month, year, "billid");

                // Filter Expenses
                string expensesQuery = @"SELECT extra_expense_id, name, amount, duedate AS [date], category, type, length, description 
                                FROM dbo.extra_expenses 
                                WHERE MONTH(duedate) = @month AND YEAR(duedate) = @year 
                                ORDER BY duedate DESC";
                LoadFilteredData(gridExpenses, expensesQuery, con, month, year, "extra_expense_id");

                // Filter Investments
                string investmentsQuery = @"SELECT investments_id, name, amount, [date], category, length, notes 
                                   FROM dbo.investments 
                                   WHERE MONTH([date]) = @month AND YEAR([date]) = @year 
                                   ORDER BY [date] DESC";
                LoadFilteredData(gridInvestments, investmentsQuery, con, month, year, "investments_id");

                // Filter Savings
                string savingsQuery = @"SELECT savings_id, name, amount, length, [date], notes 
                               FROM dbo.savings 
                               WHERE MONTH([date]) = @month AND YEAR([date]) = @year 
                               ORDER BY [date] DESC";
                LoadFilteredData(gridSavings, savingsQuery, con, month, year, "savings_id");

                UpdateTotal();
                MessageBox.Show($"Filtered to show records from {month}/{year}", "Filter Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Filter failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadFilteredData(DataGridView grid, string query, SqlConnection con, int month, int year, string idColumn)
        {
            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@month", month);
            cmd.Parameters.AddWithValue("@year", year);

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            grid.DataSource = dt;

            if (grid.Columns.Contains(idColumn))
                grid.Columns[idColumn].Visible = false;

            FormatColumnHeaders(grid);
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
                Font = new System.Drawing.Font(Font, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            Controls.Add(lbl);

            grid.Location = location;
            grid.Size = new Size(width, height);
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false; // single row selection per grid
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowHeadersVisible = false;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.EnableHeadersVisualStyles = false;


            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 150, 150);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(Font, FontStyle.Bold);


            grid.DataBindingComplete += (s, e) => { FormatGrid(grid); UpdateTotal(); };
            grid.CellValueChanged += (s, e) => UpdateTotal();
            grid.RowsRemoved += (s, e) => UpdateTotal();


            AttachSingleSelectionBehavior(grid);
        }


        private void AttachSingleSelectionBehavior(DataGridView sourceGrid)
        {

            sourceGrid.MultiSelect = false;
            sourceGrid.ClearSelection();

            sourceGrid.SelectionChanged += (s, e) =>
            {
                try
                {

                    if (sourceGrid.SelectedRows.Count > 0)
                    {
                        if (!ReferenceEquals(sourceGrid, gridBills)) gridBills.ClearSelection();
                        if (!ReferenceEquals(sourceGrid, gridExpenses)) gridExpenses.ClearSelection();
                        if (!ReferenceEquals(sourceGrid, gridInvestments)) gridInvestments.ClearSelection();
                        if (!ReferenceEquals(sourceGrid, gridSavings)) gridSavings.ClearSelection();
                    }
                }
                catch
                {

                }
            };
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
            EmergencyFundData();
            EditMonthlyAllowance();
        }


        private void AllPayments_Paint(object? sender, PaintEventArgs e)
        {
            using var brush = new LinearGradientBrush(ClientRectangle, Color.LightCoral, Color.White, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }
        private void BillsDataGrid()
        {
            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();
                string query = "SELECT billid, name, amount, [date], type, length, description FROM dbo.bills ORDER BY [date] DESC";
                var da = new SqlDataAdapter(query, con);
                var dt = new DataTable();
                da.Fill(dt);
                gridBills.DataSource = dt;

                // Hide ID column and format headers
                if (gridBills.Columns.Contains("billid"))
                    gridBills.Columns["billid"].Visible = false;
                FormatColumnHeaders(gridBills);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bills error: " + ex.Message);
            }
        }

        private void InvestmentsDataGrid()
        {
            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();
                string query = "SELECT investments_id, name, amount, [date], category, length, notes FROM dbo.investments ORDER BY [date] DESC";
                var da = new SqlDataAdapter(query, con);
                var dt = new DataTable();
                da.Fill(dt);
                gridInvestments.DataSource = dt;

                // Hide ID column and format headers
                if (gridInvestments.Columns.Contains("investments_id"))
                    gridInvestments.Columns["investments_id"].Visible = false;
                FormatColumnHeaders(gridInvestments);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Investments error: " + ex.Message);
            }
        }

        private void ExtraExpenseDataGrid()
        {
            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();
                string query = "SELECT extra_expense_id, name, amount, duedate AS [date], category, type, length, description FROM dbo.extra_expenses ORDER BY duedate DESC";
                var da = new SqlDataAdapter(query, con);
                var dt = new DataTable();
                da.Fill(dt);
                gridExpenses.DataSource = dt;

                // Hide ID column and format headers
                if (gridExpenses.Columns.Contains("extra_expense_id"))
                    gridExpenses.Columns["extra_expense_id"].Visible = false;
                FormatColumnHeaders(gridExpenses);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Extra expenses error: " + ex.Message);
            }
        }

        private void SavingsDataGrid()
        {
            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();
                string query = "SELECT savings_id, name, amount, length, [date], notes FROM dbo.savings ORDER BY [date] DESC";
                var da = new SqlDataAdapter(query, con);
                var dt = new DataTable();
                da.Fill(dt);
                gridSavings.DataSource = dt;

                // Hide ID column and format headers
                if (gridSavings.Columns.Contains("savings_id"))
                    gridSavings.Columns["savings_id"].Visible = false;
                FormatColumnHeaders(gridSavings);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Savings error: " + ex.Message);
            }
        }

        private void FormatColumnHeaders(DataGridView grid)
        {
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (!col.Visible) continue;

                // Capitalize first letter of each word
                col.HeaderText = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(col.HeaderText.ToLower());
            }
        }

        private void FormatGrid(DataGridView grid)
        {

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

            if (txtGrandTotal != null)
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

        private DataGridView? GetActiveGridWithSelection()
        {
            if (gridBills.SelectedRows.Count > 0) return gridBills;
            if (gridExpenses.SelectedRows.Count > 0) return gridExpenses;
            if (gridInvestments.SelectedRows.Count > 0) return gridInvestments;
            if (gridSavings.SelectedRows.Count > 0) return gridSavings;
            return null;
        }

        private string GetTableNameForGrid(DataGridView grid)
        {
            if (grid == gridBills) return "bills";
            if (grid == gridExpenses) return "extra_expenses";
            if (grid == gridInvestments) return "investments";
            if (grid == gridSavings) return "savings";
            return string.Empty;
        }

        private string GetPrimaryKeyColumnForGrid(DataGridView grid)
        {
            if (grid.Columns.Contains("billid")) return "billid";
            if (grid.Columns.Contains("extra_expense_id")) return "extra_expense_id";
            if (grid.Columns.Contains("investments_id")) return "investments_id";
            if (grid.Columns.Contains("savings_id")) return "savings_id";
            // fallbacks
            if (grid == gridBills) return "billid";
            if (grid == gridExpenses) return "extra_expense_id";
            if (grid == gridInvestments) return "investments_id";
            if (grid == gridSavings) return "savings_id";
            return string.Empty;
        }

        private void DeleteSelected()
        {
            var grid = GetActiveGridWithSelection();
            if (grid == null)
            {
                MessageBox.Show("Select a row in one of the grids first.", "No selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string table = GetTableNameForGrid(grid);
            string pk = GetPrimaryKeyColumnForGrid(grid);
            if (string.IsNullOrEmpty(table) || string.IsNullOrEmpty(pk))
            {
                MessageBox.Show("Could not determine table/primary key for the selected grid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            var ids = grid.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(r =>
                {
                    var v = r.Cells[pk].Value;
                    if (v == null || v == DBNull.Value) return (int?)null;
                    try { return Convert.ToInt32(v); } catch { return (int?)null; }
                })
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToList();

            if (ids.Count == 0)
            {
                MessageBox.Show("Selected row(s) do not contain primary key values.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show($"Are you sure you want to permanently delete {ids.Count} record(s)?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();
                using var tx = con.BeginTransaction();
                using var cmd = con.CreateCommand();
                cmd.Transaction = tx;

                var paramNames = ids.Select((id, idx) => "@id" + idx).ToArray();
                cmd.CommandText = $"DELETE FROM dbo.[{table}] WHERE {pk} IN ({string.Join(", ", paramNames)})";

                for (int i = 0; i < ids.Count; i++)
                    cmd.Parameters.AddWithValue(paramNames[i], ids[i]);

                int rows = cmd.ExecuteNonQuery();
                tx.Commit();

                MessageBox.Show(rows > 0 ? $"Deleted {rows} row(s)." : "No rows deleted.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditSelected()
        {
            var grid = GetActiveGridWithSelection();
            if (grid == null)
            {
                MessageBox.Show("Select a row in one of the grids first.", "No selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string table = GetTableNameForGrid(grid);
            string pk = GetPrimaryKeyColumnForGrid(grid);
            if (string.IsNullOrEmpty(table) || string.IsNullOrEmpty(pk))
            {
                MessageBox.Show("Could not determine table/primary key for the selected grid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var row = grid.SelectedRows[0];
            var idVal = row.Cells[pk].Value;
            if (idVal == null)
            {
                MessageBox.Show("Selected row does not contain a primary key value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // gather editable fields present in the grid
            var fields = new[] { "name", "amount", "date", "category", "type", "length", "notes", "description" };
            var values = fields.Where(f => grid.Columns.Contains(f)).ToDictionary(f => f, f => row.Cells[f].Value);

            using var dlg = new EditRecordDialog(values);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            var newValues = dlg.Values; // dictionary of updated values

            if (newValues.Count == 0)
            {
                MessageBox.Show("No changes to save.", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();
                using var cmd = con.CreateCommand();

                var setClauses = new List<string>();
                foreach (var kv in newValues)
                {
                    setClauses.Add($"[{kv.Key}] = @{kv.Key}");
                    cmd.Parameters.AddWithValue("@" + kv.Key, kv.Value ?? DBNull.Value);
                }

                cmd.CommandText = $"UPDATE dbo.[{table}] SET {string.Join(", ", setClauses)} WHERE {pk} = @id";
                cmd.Parameters.AddWithValue("@id", idVal);

                int rows = cmd.ExecuteNonQuery();
                MessageBox.Show(rows > 0 ? "Updated." : "No rows updated.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class EditRecordDialog : Form
    {
        private readonly Dictionary<string, object?> initial;
        public readonly Dictionary<string, object?> Values = new();

        private readonly TextBox txtName = new() { Width = 300 };
        private readonly TextBox txtAmount = new() { Width = 120 };
        private readonly DateTimePicker dtDate = new() { Format = DateTimePickerFormat.Short };
        private readonly TextBox txtCategory = new() { Width = 200 };
        private readonly TextBox txtType = new() { Width = 120 };
        private readonly TextBox txtLength = new() { Width = 120 };
        private readonly TextBox txtNotes = new() { Width = 300, Multiline = true, Height = 80 };

        public EditRecordDialog(Dictionary<string, object?> values)
        {
            initial = values;
            Text = "Edit Record";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(420, 380);
            MaximizeBox = false;
            MinimizeBox = false;

            var y = 12;
            void AddLabel(string text) => Controls.Add(new Label { Text = text, Location = new Point(12, y), AutoSize = true });
            void IncY(int delta = 28) => y += delta;

            if (values.ContainsKey("name"))
            {
                AddLabel("Name");
                txtName.Location = new Point(12, y + 18);
                txtName.Text = Convert.ToString(values["name"]) ?? string.Empty;
                Controls.Add(txtName);
                IncY(60);
            }

            if (values.ContainsKey("amount"))
            {
                AddLabel("Amount");
                txtAmount.Location = new Point(12, y + 18);
                txtAmount.Text = Convert.ToString(values["amount"], CultureInfo.InvariantCulture) ?? string.Empty;
                Controls.Add(txtAmount);
                IncY(40);
            }

            if (values.ContainsKey("date"))
            {
                AddLabel("Date");
                dtDate.Location = new Point(12, y + 18);
                if (values["date"] is DateTime dt) dtDate.Value = dt.Date;
                Controls.Add(dtDate);
                IncY(40);
            }

            if (values.ContainsKey("category"))
            {
                AddLabel("Category");
                txtCategory.Location = new Point(12, y + 18);
                txtCategory.Text = Convert.ToString(values["category"]) ?? string.Empty;
                Controls.Add(txtCategory);
                IncY(40);
            }

            if (values.ContainsKey("type"))
            {
                AddLabel("Type");
                txtType.Location = new Point(12, y + 18);
                txtType.Text = Convert.ToString(values["type"]) ?? string.Empty;
                Controls.Add(txtType);
                IncY(40);
            }

            if (values.ContainsKey("length"))
            {
                AddLabel("Length");
                txtLength.Location = new Point(12, y + 18);
                txtLength.Text = Convert.ToString(values["length"]) ?? string.Empty;
                Controls.Add(txtLength);
                IncY(40);
            }

            if (values.ContainsKey("notes") || values.ContainsKey("description"))
            {
                AddLabel("Notes");
                txtNotes.Location = new Point(12, y + 18);
                var noteVal = values.ContainsKey("notes") ? values["notes"] : values.GetValueOrDefault("description");
                txtNotes.Text = Convert.ToString(noteVal) ?? string.Empty;
                Controls.Add(txtNotes);
                IncY(100);
            }

            var btnSave = new Button { Text = "Save", Location = new Point(240, ClientSize.Height - 44), Size = new Size(80, 30) };
            var btnCancel = new Button { Text = "Cancel", Location = new Point(330, ClientSize.Height - 44), Size = new Size(80, 30) };
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (Controls.Contains(txtAmount) && !string.IsNullOrWhiteSpace(txtAmount.Text))
            {
                if (!decimal.TryParse(txtAmount.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out _)
                    && !decimal.TryParse(txtAmount.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out _))
                {
                    MessageBox.Show("Amount must be a number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (Controls.Contains(txtName)) Values["name"] = txtName.Text.Trim();
            if (Controls.Contains(txtAmount)) Values["amount"] = string.IsNullOrWhiteSpace(txtAmount.Text) ? (object?)DBNull.Value : (object)decimal.Parse(txtAmount.Text, CultureInfo.CurrentCulture);
            if (Controls.Contains(dtDate)) Values["date"] = dtDate.Value.Date;
            if (Controls.Contains(txtCategory)) Values["category"] = string.IsNullOrWhiteSpace(txtCategory.Text) ? (object?)DBNull.Value : txtCategory.Text.Trim();
            if (Controls.Contains(txtType)) Values["type"] = string.IsNullOrWhiteSpace(txtType.Text) ? (object?)DBNull.Value : txtType.Text.Trim();
            if (Controls.Contains(txtLength)) Values["length"] = string.IsNullOrWhiteSpace(txtLength.Text) ? (object?)DBNull.Value : txtLength.Text.Trim();
            if (Controls.Contains(txtNotes)) Values["notes"] = string.IsNullOrWhiteSpace(txtNotes.Text) ? (object?)DBNull.Value : txtNotes.Text.Trim();

            DialogResult = DialogResult.OK;
        }
    }
}
