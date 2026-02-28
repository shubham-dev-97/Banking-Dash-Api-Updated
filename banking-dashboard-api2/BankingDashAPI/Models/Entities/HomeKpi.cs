namespace BankingDashAPI.Models.Entities
{
    public class HomeKpi
    {
        public int TotalLoanAccounts { get; set; }
        public int TotalLoanCustomers { get; set; }
        public decimal TotalLoanOutstanding { get; set; }
        public decimal AvgLoanInterest { get; set; }

        public int TotalDepositAccounts { get; set; }
        public int TotalDepositCustomers { get; set; }
        public decimal TotalDepositBalance { get; set; }
        public decimal AvgDepositInterest { get; set; }

        public int ActiveLoanAccounts { get; set; }
        public int ActiveDepositAccounts { get; set; }

    }
}