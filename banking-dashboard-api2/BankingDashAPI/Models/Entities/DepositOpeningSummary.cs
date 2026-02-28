namespace BankingDashAPI.Models.Entities
{
    public class DepositOpeningSummary
    {
        public int TotalDepositOpenLast30Days { get; set; }
        public int TotalDepositAccountInBank { get; set; }
        public decimal TotalDepositAmount { get; set; }
        public decimal OpeningPercentage { get; set; }
    }
}
