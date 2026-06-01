namespace BankingDashAPI.Models.Entities
{
    public class BankKPIDashboard
    {
        public class BankKPISummary
        {
            public int TOTAL_BRANCHES { get; set; }
            public decimal TOTAL_CUSTOMERS { get; set; }
            public decimal ACTIVE_CUSTOMERS { get; set; }
            public decimal NEW_CUSTOMERS { get; set; }
            public decimal TOTAL_DEPOSIT_CR { get; set; }
            public decimal TOTAL_LOAN_CR { get; set; }
            public decimal TOTAL_RECOVERY_CR { get; set; }
            public decimal AVG_GROSS_NPA { get; set; }
            public decimal AVG_NET_NPA { get; set; }
            public decimal AVG_CASA_RATIO { get; set; }
            public decimal DIGITAL_PERCENT { get; set; }
            public decimal AVG_PERFORMANCE { get; set; }
            public decimal TOTAL_UPI_TRANSACTION_CR { get; set; }
        }

        // Result Set 2: CEO Yearly Summary
        public class CEOYearlySummary
        {
            public string FIN_YEAR { get; set; } = string.Empty;
            public int TOTAL_BRANCHES { get; set; }
            public decimal TOTAL_DEPOSIT_CR { get; set; }
            public decimal TOTAL_LOAN_CR { get; set; }
            public decimal TOTAL_RECOVERY_CR { get; set; }
            public decimal AVG_GROSS_NPA { get; set; }
            public decimal AVG_NET_NPA { get; set; }
            public decimal DIGITAL_PERCENT { get; set; }
            public decimal AVG_PERFORMANCE { get; set; }
        }

        // Result Set 3: Region Summary
        public class RegionKPISummary
        {
            public string REGION_NAME { get; set; } = string.Empty;
            public int TOTAL_BRANCHES { get; set; }
            public decimal TOTAL_DEPOSIT_CR { get; set; }
            public decimal TOTAL_LOAN_CR { get; set; }
            public decimal TOTAL_RECOVERY_CR { get; set; }
            public decimal AVG_NPA { get; set; }
            public decimal DIGITAL_PERCENT { get; set; }
            public decimal AVG_PERFORMANCE { get; set; }
        }

        // Result Set 4 & 5: Top/Bottom Branches
        public class BranchPerformanceItem
        {
            public string PBRCODE { get; set; } = string.Empty;
            public string BRANCH_NAME { get; set; } = string.Empty;
            public string REGION_NAME { get; set; } = string.Empty;
            public decimal TOTAL_DEPOSIT_ACHIEVED_CR { get; set; }
            public decimal TOTAL_LOAN_ACHIEVED_CR { get; set; }
            public decimal RECOVERY_ACHIEVED_CR { get; set; }
            public decimal GROSS_NPA_PERCENT { get; set; }
            public decimal DIGITAL_TRANSACTION_PERCENT { get; set; }
            public decimal OVERALL_ACHIEVEMENT_PERCENT { get; set; }
            public string PERFORMANCE_STATUS { get; set; } = string.Empty;
        }

        // Result Set 6: Branch Detail Grid
        public class BranchDetailGrid
        {
            public string PBRCODE { get; set; } = string.Empty;
            public string BRANCH_NAME { get; set; } = string.Empty;
            public string REGION_NAME { get; set; } = string.Empty;
            public decimal TOTAL_CUSTOMERS { get; set; }
            public decimal ACTIVE_CUSTOMERS { get; set; }
            public decimal NEW_CUSTOMERS { get; set; }
            public int STAFF_COUNT { get; set; }
            public decimal TOTAL_DEPOSIT_TARGET_CR { get; set; }
            public decimal TOTAL_DEPOSIT_ACHIEVED_CR { get; set; }
            public decimal CASA_TARGET_CR { get; set; }
            public decimal CASA_ACHIEVED_CR { get; set; }
            public decimal TERM_DEPOSIT_TARGET_CR { get; set; }
            public decimal TERM_DEPOSIT_ACHIEVED_CR { get; set; }
            public decimal CASA_RATIO_PERCENT { get; set; }
            public decimal TOTAL_LOAN_TARGET_CR { get; set; }
            public decimal TOTAL_LOAN_ACHIEVED_CR { get; set; }
            public decimal MSME_LOAN_CR { get; set; }
            public decimal GOLD_LOAN_CR { get; set; }
            public decimal RECOVERY_TARGET_CR { get; set; }
            public decimal RECOVERY_ACHIEVED_CR { get; set; }
            public decimal GROSS_NPA_PERCENT { get; set; }
            public decimal NET_NPA_PERCENT { get; set; }
            public decimal MOBILE_BANKING_CUSTOMERS { get; set; }
            public decimal INTERNET_BANKING_CUSTOMERS { get; set; }
            public decimal UPI_TRANSACTION_CR { get; set; }
            public decimal DIGITAL_TRANSACTION_PERCENT { get; set; }
            public decimal OVERALL_ACHIEVEMENT_PERCENT { get; set; }
            public string PERFORMANCE_STATUS { get; set; } = string.Empty;
            public int BRANCH_RANK { get; set; }
        }

        // Result Set 7: Map Dataset
        public class BranchMapData
        {
            public string PBRCODE { get; set; } = string.Empty;
            public string BRANCH_NAME { get; set; } = string.Empty;
            public string REGION_NAME { get; set; } = string.Empty;
            public decimal? LATITUDE { get; set; }
            public decimal? LONGITUDE { get; set; }
            public decimal TOTAL_DEPOSIT_ACHIEVED_CR { get; set; }
            public decimal TOTAL_LOAN_ACHIEVED_CR { get; set; }
            public decimal RECOVERY_ACHIEVED_CR { get; set; }
            public decimal GROSS_NPA_PERCENT { get; set; }
            public decimal DIGITAL_TRANSACTION_PERCENT { get; set; }
            public decimal OVERALL_ACHIEVEMENT_PERCENT { get; set; }
            public string PERFORMANCE_STATUS { get; set; } = string.Empty;
            public string GOOGLE_MAP_LOCATION { get; set; } = string.Empty;
        }

        // Result Set 8: Trend Analysis
        public class KPIYearlyTrend
        {
            public string FIN_YEAR { get; set; } = string.Empty;
            public decimal TOTAL_DEPOSIT_CR { get; set; }
            public decimal TOTAL_LOAN_CR { get; set; }
            public decimal TOTAL_RECOVERY_CR { get; set; }
            public decimal AVG_NPA { get; set; }
            public decimal DIGITAL_PERCENT { get; set; }
            public decimal AVG_PERFORMANCE { get; set; }
        }

        // Result Set 9: Actual vs Projection
        public class ActualVsProjection
        {
            public string YEAR_TYPE { get; set; } = string.Empty;
            public decimal TOTAL_DEPOSIT_CR { get; set; }
            public decimal TOTAL_LOAN_CR { get; set; }
            public decimal TOTAL_RECOVERY_CR { get; set; }
            public decimal AVG_NPA { get; set; }
            public decimal DIGITAL_PERCENT { get; set; }
            public decimal AVG_PERFORMANCE { get; set; }
        }

        // Combined Response
        public class BankKPIDashboardResponse
        {
            public BankKPISummary Summary { get; set; } = new();
            public List<CEOYearlySummary> YearlySummary { get; set; } = new();
            public List<RegionKPISummary> RegionSummary { get; set; } = new();
            public List<BranchPerformanceItem> TopBranches { get; set; } = new();
            public List<BranchPerformanceItem> BottomBranches { get; set; } = new();
            public List<BranchDetailGrid> BranchGrid { get; set; } = new();
            public List<BranchMapData> MapData { get; set; } = new();
            public List<KPIYearlyTrend> TrendAnalysis { get; set; } = new();
            public List<ActualVsProjection> ActualVsProjection { get; set; } = new();
        }

        // Request Model
        public class BankKPIRequest
        {
            public string? FIN_YEAR { get; set; }
            public string? YEAR_TYPE { get; set; }
            public string? REGION_NAME { get; set; }
            public int? PBRCODE { get; set; }
            public int TOP_RECORDS { get; set; } = 10;
        }
    }
}
