namespace BankingDashAPI.Models.Entities
{
    public class LoanPortfolioOverview
    {
        public decimal Total_Loan_Amount { get; set; }
        public decimal Total_Outstanding { get; set; }
        public decimal Total_Overdue { get; set; }
        public decimal Avg_Interest_Rate { get; set; }
        public int Total_Accounts { get; set; }
        public int Active_Accounts { get; set; }
        public int Overdue_Accounts { get; set; }
        public decimal Avg_Loan_Size { get; set; }
    }
}
