using System;

namespace BankingDashAPI.Models.Entities
{
    public class RbiLoanAuditDump
    {
        public DateTime reporT_DATE { get; set; }
        public long brancH_ID { get; set; }
        public long loaN_ACCOUNT_NO { get; set; }
        public string customeR_ID { get; set; } = string.Empty;
        public string borroweR_NAME { get; set; } = string.Empty;
        public string customeR_TYPE { get; set; } = string.Empty;
        public DateTime? dob { get; set; }
        public string gender { get; set; } = string.Empty;
        public string paN_NO { get; set; } = string.Empty;
        public string ckyC_ID { get; set; } = string.Empty;
        public string gsT_NO { get; set; } = string.Empty;
        public string addresS_LINE1 { get; set; } = string.Empty;
        public string addresS_LINE2 { get; set; } = string.Empty;
        public string addresS_LINE3 { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string pincode { get; set; } = string.Empty;
        public string mobilE_NO { get; set; } = string.Empty;
        public string emaiL_ID { get; set; } = string.Empty;
        public string loaN_PRODUCT_CODE { get; set; } = string.Empty;
        public string loaN_PURPOSE { get; set; } = string.Empty;
        public string prioritY_SECTOR { get; set; } = string.Empty;
        public string psL_CODE { get; set; } = string.Empty;
        public string customeR_SEGMENT { get; set; } = string.Empty;
        public DateTime? sanctioN_DATE { get; set; }
        public DateTime? disbursemenT_DATE { get; set; }
        public decimal sanctioN_AMOUNT { get; set; }
        public string sanctioneD_BY { get; set; } = string.Empty;
        public DateTime? accounT_OPEN_DATE { get; set; }
        public DateTime? maturitY_DATE { get; set; }
        public decimal outstandinG_AMOUNT { get; set; }
        public decimal interesT_RECEIVABLE { get; set; }
        public decimal emI_AMOUNT { get; set; }
        public decimal securitY_VALUE { get; set; }
        public decimal roi { get; set; }
        public string repaymenT_MODE { get; set; } = string.Empty;
        public decimal tenure { get; set; }
        public string asseT_CLASSIFICATION { get; set; } = string.Empty;
        public string internaL_RATING { get; set; } = string.Empty;
        public string weakeR_SECTION_FLAG { get; set; } = string.Empty;
        public string securitY_TYPE { get; set; } = string.Empty;
        public decimal overduE_AMOUNT { get; set; }
        public decimal dayS_PAST_DUE { get; set; }
        public DateTime? datE_OF_DEFAULT { get; set; }
        public decimal provisioN_PERCENT { get; set; }
        public decimal secureD_PROVISION { get; set; }
        public decimal unsecureD_PROVISION { get; set; }
        public decimal totaL_PROVISION { get; set; }
        public string rbI_ASSET_CLASS { get; set; } = string.Empty;
        public string reporT_TYPE { get; set; } = string.Empty;
    }
}