namespace BankingDashAPI.Models.Entities
{
    public class NPASummary
    {
        public int TotalNPAOpenLast30Days { get; set; }
        public int TotalNPAAccountInBank { get; set; }
        public decimal TotalNPAAmount { get; set; }
        public decimal OpeningPercentage { get; set; }
    }
}
