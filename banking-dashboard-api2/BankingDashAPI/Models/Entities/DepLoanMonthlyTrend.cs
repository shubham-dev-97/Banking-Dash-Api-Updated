namespace BankingDashAPI.Models.Entities
{
    public class DepLoanMonthlyTrend
    {
        public DateTime MONTH_END { get; set; }
        public decimal DepositBal { get; set; }
        public decimal LoanBal { get; set; }
        public decimal CD_RATIO_PERCENT { get; set; }
        public string Deposit_tag { get; set; } = string.Empty;
        public string Loan_tag { get; set; } = string.Empty;
    }
}
