namespace BankingDashAPI.Models.Entities
{
    public class DepositAnalysis
    {
        public string? BranchCode { get; set; }
        public string? ProductCode { get; set; }
        public string? SchemeCode { get; set; }
        public string? CustomerCategory { get; set; }
        public string? AccountStatus { get; set; }
        public string? CustomerGender { get; set; }

        public int? AccountCount { get; set; }
        public decimal? TotalBalance { get; set; }
        public decimal? TotalDepositAmount { get; set; }
        public decimal? AvgInterestRate { get; set; }

        public int? OpenYear { get; set; }
        public int? OpenMonth { get; set; }
    }
}