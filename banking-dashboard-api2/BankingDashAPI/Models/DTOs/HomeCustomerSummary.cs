namespace BankingDashAPI.Models.DTOs
{
    public class HomeCustomerSummary
    {
        public int TotalDepositCustomers { get; set; }
        public int TotalLoanCustomers { get; set; }
        public int TotalCustomers { get; set; }
        public int NpaCustomers { get; set; }
    }
}
