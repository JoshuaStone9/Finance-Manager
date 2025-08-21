using System;
using System.Linq;
using System.Windows.Forms;

namespace FM
{
    public partial class ExtraExpensesRecord : Form
    {
        private readonly BindingSource _bs = new();

        // Add this declaration for gridExpenses
        private DataGridView gridExpenses;

        // Add these declarations inside the ExtraExpensesRecord class, near other field declarations
        private Button btnDeleteExpenses;
        private Button btnCloseExpenses;

        public ExtraExpensesRecord()
        {
            InitializeComponent();

            // Initialize buttons before using them
            btnDeleteExpenses = new Button
            {
                Text = "Delete Expenses",
                Dock = DockStyle.Bottom
            };
            btnCloseExpenses = new Button
            {
                Text = "Close",
                Dock = DockStyle.Bottom
            };

            Controls.Add(btnDeleteExpenses);
            Controls.Add(btnCloseExpenses);

            // Initialize gridExpenses before using it
            gridExpenses = new DataGridView
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(gridExpenses);

            SetupGrid();

            btnDeleteExpenses.Click += btnDeleteExpenses_Click;
            btnCloseExpenses.Click += (s, e) => Close();
        }

        private void SetupGrid()
        {
            // Binding to store (make sure ExtraExpenseStore.Expenses is BindingList<ExtraExpenseRecord>)
            _bs.DataSource = ExtraExpenseStore.Expenses;
            gridExpenses.AutoGenerateColumns = false;
            gridExpenses.DataSource = _bs;

            gridExpenses.Columns.Clear();

            // Expense ID
            gridExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ExtraExpenseRecord.ExpenseId),
                HeaderText = "Expense ID",
                MinimumWidth = 120,
                ReadOnly = true
            });

            // Name
            gridExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ExtraExpenseRecord.Name),
                HeaderText = "Name",
                MinimumWidth = 160,
                ReadOnly = true
            });

            // Amount (currency)
            gridExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ExtraExpenseRecord.Amount),
                HeaderText = "Amount",
                MinimumWidth = 110,
                ReadOnly = true,
                DefaultCellStyle = { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            // Category
            gridExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ExtraExpenseRecord.Category),
                HeaderText = "Category",
                MinimumWidth = 120,
                ReadOnly = true
            });

            // Type
            gridExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ExtraExpenseRecord.Type),
                HeaderText = "Type",
                MinimumWidth = 120,
                ReadOnly = true
            });

            // Frequency
            gridExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ExtraExpenseRecord.Frequency),
                HeaderText = "Frequency",
                MinimumWidth = 120,
                ReadOnly = true
            });

            // Date Incurred
            gridExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ExtraExpenseRecord.DateIncurred),
                HeaderText = "Date",
                MinimumWidth = 110,
                ReadOnly = true,
                DefaultCellStyle = { Format = "d", Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            // Notes
            gridExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ExtraExpenseRecord.Notes),
                HeaderText = "Notes",
                MinimumWidth = 200,
                ReadOnly = true,
                DefaultCellStyle = { WrapMode = DataGridViewTriState.True }
            });

            // Grid defaults
            gridExpenses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridExpenses.MultiSelect = true;
            gridExpenses.ReadOnly = true;
            gridExpenses.AllowUserToAddRows = false;
            gridExpenses.AllowUserToDeleteRows = false;
            gridExpenses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridExpenses.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            gridExpenses.RowHeadersVisible = false;

            // Optional: sort by date descending
            if (gridExpenses.Columns[nameof(ExtraExpenseRecord.DateIncurred)] is DataGridViewColumn dateCol)
                gridExpenses.Sort(dateCol, System.ComponentModel.ListSortDirection.Descending);
        }

        private void btnDeleteExpenses_Click(object? sender, EventArgs e)
        {
            if (gridExpenses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select at least one expense to delete.");
                return;
            }

            if (MessageBox.Show($"Delete {gridExpenses.SelectedRows.Count} expense(s)?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var toRemove = gridExpenses.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(r => r.DataBoundItem as ExtraExpenseRecord)
                .Where(x => x != null)
                .ToList();

            foreach (var rec in toRemove)
                ExtraExpenseStore.Expenses.Remove(rec!);
        }

        // Add this method if missing
        private void InitializeComponent()
        {
            // If you have designer code, move it here.
            // Otherwise, leave empty to avoid CS0103.
        }
    }

    // Your data model for extra expenses
    public class ExtraExpenseRecord
    {
        public string ExpenseId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Frequency { get; set; }
        public DateTime DateIncurred { get; set; }
        public string Notes { get; set; }
    }
}
