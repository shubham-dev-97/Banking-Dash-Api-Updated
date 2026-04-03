namespace BankingDashAPI.Models.Entities
{
    public class DepositPortfolioOverview
    {
        public decimal Total_Balance { get; set; }
        public int Total_Accounts { get; set; }
        public decimal Avg_Balance { get; set; }
        public decimal Avg_Interest_Rate { get; set; }
        public int Active_Accounts { get; set; }
        public int Dormant_Accounts { get; set; }
        public int Closed_Accounts { get; set; }
        public decimal Avg_Account_Size { get; set; }
    }
}
