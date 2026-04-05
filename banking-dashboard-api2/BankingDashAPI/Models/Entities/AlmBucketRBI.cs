namespace BankingDashAPI.Models.Entities
{
    public class AlmBucketRBI
    {
        public string RBI_BUCKET { get; set; } = string.Empty;
        public int NO_OF_ACCOUNTS { get; set; }
        public decimal OUTSTANDING_BALANCE { get; set; }
        public decimal MATURITY_AMOUNT { get; set; }
    }
}
