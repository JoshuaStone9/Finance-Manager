

    public class ExtraExpenseRecord
    {
        public string ExpenseId { get; set; } = "";
        public string Name { get; set; } = "";
        public decimal Amount { get; set; }
        public string Category { get; set; } = "Other";
        public string Type { get; set; } = "One-off";  // or Recurring
        public string Frequency { get; set; } = "N/A";      // when Recurring
        public DateTime DateIncurred { get; set; } = DateTime.Today;
        public string Notes { get; set; } = "";
    }

