namespace BankingDashAPI.Models.Entities
{
    public class GLDashboardSummary
    {
        public decimal Total_Assets { get; set; }
        public decimal Total_Liabilities { get; set; }
        public decimal Total_Income { get; set; }
        public decimal Total_Expense { get; set; }
        public decimal Total_Debit { get; set; }
        public decimal Total_Credit { get; set; }
        public decimal Net_Profit { get; set; }
        public decimal Net_Position { get; set; }
    }
}
