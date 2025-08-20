using System;

namespace FM
{
    public class InvestmentRecord
    {
        public string Investment_ID { get; set; } = "";
        public string Name { get; set; } = "";
        public decimal Amount { get; set; }
        public string Category { get; set; } = "";   // e.g., Stocks, ETF, Crypto, Real Estate
        public string Length { get; set; } = "One-time";
        public DateTime Date { get; set; }           // purchase/start date
        public string Description { get; set; } = "";
    }
}
