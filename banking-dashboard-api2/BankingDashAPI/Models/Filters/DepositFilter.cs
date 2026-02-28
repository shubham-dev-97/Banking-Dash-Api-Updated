namespace BankingDashAPI.Models.Filters
{
    public class DepositFilter
    {
        public string? BranchCode { get; set; }
        public string? ProductCode { get; set; }
        public string? SchemeCode { get; set; }
        public string? CustomerCategory { get; set; }
        public string? AccountStatus { get; set; }
        public string? Gender { get; set; }

        public int? Year { get; set; }
        public int? Month { get; set; }
    }
}
