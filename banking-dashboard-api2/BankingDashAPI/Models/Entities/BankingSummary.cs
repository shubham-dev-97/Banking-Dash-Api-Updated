namespace BankingDashAPI.Models.Entities
{
    public class BankingSummary
    {
        public string? BranchCode { get; set; }

        public int? TotalLoanAccounts { get; set; }
        public decimal? TotalLoanAmount { get; set; }

        public int? TotalDepositAccounts { get; set; }
        public decimal? TotalDepositAmount { get; set; }

        public decimal? NetPosition { get; set; }
        public decimal? LoanDepositRatio { get; set; }
    }
}