namespace BankingDashAPI.Models.Entities
{
    public class LoanTrend
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalOutstanding { get; set; }
        public decimal TotalSanctioned { get; set; }
        public int AccountCount { get; set; }
        public decimal AverageLoanSize { get; set; }
    }
}
