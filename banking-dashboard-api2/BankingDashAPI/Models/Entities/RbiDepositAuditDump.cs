namespace BankingDashAPI.Models.Entities
{
    public class RbiDepositAuditDump
    {
        // Bank / Account
        public DateTime REPORT_DATE { get; set; }
        public string BRANCH_ID { get; set; } = string.Empty;
        public string GL_CODE { get; set; } = string.Empty;
        public string ACCOUNT_NO { get; set; } = string.Empty;
        public string ACCOUNT_NAME { get; set; } = string.Empty;
        public string SCHEME_CODE { get; set; } = string.Empty;
        public string PRODUCT_TYPE { get; set; } = string.Empty;

        // Customer Details
        public string CUSTOMER_ID { get; set; } = string.Empty;
        public string CUSTOMER_NAME { get; set; } = string.Empty;
        public string FATHER_NAME { get; set; } = string.Empty;
        public string MOTHER_NAME { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public string GENDER { get; set; } = string.Empty;
        public string CUSTOMER_CATEGORY { get; set; } = string.Empty;
        public string CUSTOMER_CLASSIFICATION { get; set; } = string.Empty;
        public string CUSTOMER_STATUS { get; set; } = string.Empty;

        // KYC / AML
        public string KYC_STATUS { get; set; } = string.Empty;
        public string CKYC_NUMBER { get; set; } = string.Empty;
        public string PAN { get; set; } = string.Empty;
        public string FATCA_STATUS { get; set; } = string.Empty;
        public string AML_RISK_CATEGORY { get; set; } = string.Empty;
        public string FORM15G_H_STATUS { get; set; } = string.Empty;

        // Account Status
        public string ACCOUNT_STATUS { get; set; } = string.Empty;
        public string INOPERATIVE_FLAG { get; set; } = string.Empty;
        public string DORMANT_STATUS { get; set; } = string.Empty;
        public string LIEN_STATUS { get; set; } = string.Empty;

        // Important Dates
        public DateTime? ACCOUNT_OPEN_DATE { get; set; }
        public DateTime? ACCOUNT_CLOSE_DATE { get; set; }
        public DateTime? LAST_TRANSACTION_DATE { get; set; }
        public DateTime? DEPOSIT_START_DATE { get; set; }
        public DateTime? MATURITY_DATE { get; set; }
        public DateTime? DEPOSIT_END_DATE { get; set; }

        // Financial Details
        public decimal CURRENT_BALANCE { get; set; }
        public decimal DEPOSIT_AMOUNT { get; set; }
        public decimal MATURITY_AMOUNT { get; set; }
        public decimal TOTAL_CREDITS { get; set; }
        public decimal TOTAL_DEBITS { get; set; }
        public decimal AVG_QUARTERLY_BALANCE { get; set; }

        // Interest Details
        public decimal INTEREST_RATE { get; set; }
        public string INTEREST_PAYOUT_MODE { get; set; } = string.Empty;

        // Transaction
        public int TOTAL_TRANSACTIONS { get; set; }

        // Address
        public string ADDRESS_LINE1 { get; set; } = string.Empty;
        public string ADDRESS_LINE2 { get; set; } = string.Empty;
        public string ADDRESS_LINE3 { get; set; } = string.Empty;
        public string CITY { get; set; } = string.Empty;
        public string PIN_CODE { get; set; } = string.Empty;
        public string STATE_CODE { get; set; } = string.Empty;

        // Contact
        public string MOBILE_NO { get; set; } = string.Empty;
        public string EMAIL_ID { get; set; } = string.Empty;

        // Membership
        public string MEMBER_TYPE { get; set; } = string.Empty;
        public string MEMBER_ID { get; set; } = string.Empty;

        // Deposit Receipt
        public string DEPOSIT_RECEIPT_NO { get; set; } = string.Empty;
        public string DEPOSIT_TYPE_CODE { get; set; } = string.Empty;
        public string DEPOSIT_STATUS { get; set; } = string.Empty;
        public string DEPOSIT_TENURE { get; set; } = string.Empty;

        // RBI Derived Fields
        public string DEPOSIT_TYPE { get; set; } = string.Empty;
        public string DEPOSIT_SIZE_FLAG { get; set; } = string.Empty;
        public string KYC_RISK_FLAG { get; set; } = string.Empty;
        public string AML_RISK_LEVEL { get; set; } = string.Empty;
        public string ACCOUNT_ACTIVITY_STATUS { get; set; } = string.Empty;
        public string AUDIT_REMARK { get; set; } = string.Empty;
    }
}
