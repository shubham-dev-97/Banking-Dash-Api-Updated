namespace BankingDashAPI.Models.Filters
{
    public class LoanFilter
    {
        public string? BranchCode { get; set; }
        public string? SchemeCode { get; set; }
        public string? Purpose { get; set; }
        public string? Segment { get; set; }
        public string? PrioritySector { get; set; }
        public string? SecureType { get; set; }
        public string? AccountStatus { get; set; }

        public int? Year { get; set; }
        public int? Month { get; set; }
    }
}
