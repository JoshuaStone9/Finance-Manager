using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FM
{
    public partial class BillsRecord : Form
    {
        private readonly BindingSource _bs = new();
        private DataTable _dt = new();

        public BillsRecord()
        {
            InitializeComponent();
            SetupGrid();

            btnDeleteBills.Click += btnDeleteBills_Click;
            btnCloseBills.Click += (s, e) => Close();

            Load += BillsRecord_Load;
        }

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

        private void BillsRecord_Load(object? sender, EventArgs e)
        {
            try
            {
                LoadBillsFromDb();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load bills: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupGrid()
        {
            gridBills.AutoGenerateColumns = true;
            gridBills.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridBills.MultiSelect = true;
            gridBills.ReadOnly = true;
            gridBills.AllowUserToAddRows = false;
            gridBills.AllowUserToDeleteRows = false;
            gridBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            gridBills.DataSource = _bs;
        }

        private void LoadBillsFromDb()
        {
            using var con = new SqlConnection(BuildConnStr());
            con.Open();

            using var da = new SqlDataAdapter("SELECT billid, name, amount, [date], type, length, description FROM dbo.bills ORDER BY [date] DESC;", con);
            var newDt = new DataTable();
            da.Fill(newDt);

            _dt = newDt;
            _bs.DataSource = _dt;
        }

        private void btnDeleteBills_Click(object? sender, EventArgs e)
        {
            if (gridBills.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select at least one bill to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Delete {gridBills.SelectedRows.Count} bill(s)?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var ids = gridBills.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(r =>
                {
                    if (r.Cells["billid"].Value == null) return (int?)null;
                    try { return Convert.ToInt32(r.Cells["billid"].Value); }
                    catch { return (int?)null; }
                })
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            if (ids.Count == 0)
            {
                MessageBox.Show("Could not determine the primary key values for the selected rows.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using var con = new SqlConnection(BuildConnStr());
                con.Open();

                using var tx = con.BeginTransaction();
                using var cmd = con.CreateCommand();
                cmd.Transaction = tx;

                var paramNames = ids.Select((id, idx) => "@id" + idx).ToArray();
                cmd.CommandText = $"DELETE FROM dbo.bills WHERE billid IN ({string.Join(", ", paramNames)})";

                for (int i = 0; i < ids.Count; i++)
                    cmd.Parameters.AddWithValue(paramNames[i], ids[i]);

                int rowsAffected = cmd.ExecuteNonQuery();
                tx.Commit();

                MessageBox.Show(rowsAffected > 0 ? $"Deleted {rowsAffected} row(s)." : "No rows deleted.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadBillsFromDb();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Delete failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
