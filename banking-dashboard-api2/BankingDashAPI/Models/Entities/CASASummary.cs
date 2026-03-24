namespace BankingDashAPI.Models.Entities
{
    public class CASASummary
    {
        public string Deposit_Type { get; set; } = string.Empty;
        public decimal Total_Balance { get; set; }
        public int Cnt { get; set; }
    }
}
