namespace BankingDashAPI.Models.Entities
{
    public class DepositTrend
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalBalance { get; set; }
        public int AccountCount { get; set; }
        public decimal AverageBalance { get; set; }
    }
}
