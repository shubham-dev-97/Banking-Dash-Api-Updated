namespace BankingDashAPI.Models.Entities
{
    public class LoanAnalysis
    {
        public string? BranchCode { get; set; }
        public string? LoanSchemeCode { get; set; }
        public string? Purpose { get; set; }
        public string? PrioritySector { get; set; }
        public string? Segment { get; set; }
        public string? SecureUnsecure { get; set; }
        public string? AccountStatus { get; set; }

        public int? LoanCount { get; set; }
        public decimal? TotalSanctionAmount { get; set; }
        public decimal? TotalOutstanding { get; set; }
        public decimal? TotalOverdueAmount { get; set; }
        public decimal? AvgInterestRate { get; set; }

        public int? DisbursementYear { get; set; }
        public int? DisbursementMonth { get; set; }
    }
}