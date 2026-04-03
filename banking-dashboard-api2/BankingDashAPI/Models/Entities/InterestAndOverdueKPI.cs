namespace BankingDashAPI.Models.Entities
{
    public class InterestAndOverdueKPI
    {
        public decimal Avg_Loan_Interest_Rate { get; set; }
        public decimal Avg_Deposit_Interest_Rate { get; set; }
        public decimal Overdue_Amount { get; set; }
        public decimal Avg_Account_Size { get; set; }
    }
}
