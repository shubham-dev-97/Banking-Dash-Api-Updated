namespace BankingDashAPI.Models.Entities
{
    public class PortfolioOverview
    {
        public decimal Total_Deposit { get; set; }
        public decimal Total_Loan { get; set; }
        public decimal Net_Position { get; set; }
        public decimal Loan_To_Deposit_Ratio { get; set; }
    }
}
