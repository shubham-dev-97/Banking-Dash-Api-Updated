namespace BankingDashAPI.Models.Entities
{
    public class MonthlyTrend
    {
        public int? Year { get; set; }
        public int? Month { get; set; }

        public decimal? TotalLoanAmount { get; set; }
        public decimal? TotalDepositAmount { get; set; }

        public decimal? NetPosition { get; set; }
        public decimal? LoanDepositRatio { get; set; }
    }
}