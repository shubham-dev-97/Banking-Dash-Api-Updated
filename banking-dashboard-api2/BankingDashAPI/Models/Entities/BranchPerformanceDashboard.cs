namespace BankingDashAPI.Models.Entities
{
    public class BranchPerformanceDashboard
    {
        public class BranchPerformanceSummary
        {
            public int TOTAL_BRANCHES { get; set; }
            public decimal TOTAL_DAILY_RECOVERY_TARGET { get; set; }
            public decimal TOTAL_DAILY_RECOVERY_ACHIEVED { get; set; }
            public decimal DAILY_RECOVERY_PERCENT { get; set; }
            public decimal TOTAL_CASA_TARGET { get; set; }
            public decimal TOTAL_CASA_ACHIEVED { get; set; }
            public decimal CASA_PERCENT { get; set; }
            public decimal TOTAL_TERM_DEPOSIT_TARGET { get; set; }
            public decimal TOTAL_TERM_DEPOSIT_ACHIEVED { get; set; }
            public decimal TERM_DEPOSIT_PERCENT { get; set; }
            public int TOTAL_NEW_CUSTOMERS { get; set; }
            public int TOTAL_MOBILE_BANKING_CUSTOMERS { get; set; }
            public decimal AVG_NPA_PERCENT { get; set; }
            public decimal AVG_OVERALL_ACHIEVEMENT { get; set; }
            public DateTime LAST_UPDATED { get; set; }
        }


        public class BranchPerformanceGrid
        {
            public int BRANCH_RANK { get; set; }
            public string PBRCODE { get; set; } = string.Empty;
            public string BRANCH_NAME { get; set; } = string.Empty;
            public string BRANCH_MANAGER { get; set; } = string.Empty;
            public decimal DAILY_RECOVERY_TARGET { get; set; }
            public decimal DAILY_RECOVERY_ACHIEVED { get; set; }
            public decimal DAILY_RECOVERY_PERCENT { get; set; }
            public decimal CASA_TARGET_CR { get; set; }
            public decimal CASA_ACHIEVED_CR { get; set; }
            public decimal CASA_PERCENT { get; set; }
            public decimal TERM_DEPOSIT_TARGET_CR { get; set; }
            public decimal TERM_DEPOSIT_ACHIEVED_CR { get; set; }
            public decimal TERM_DEPOSIT_PERCENT { get; set; }
            public int NEW_CUSTOMERS { get; set; }
            public int MOBILE_BANKING_CUSTOMERS { get; set; }
            public decimal NPA_PERCENT { get; set; }
            public decimal OVERALL_ACHIEVEMENT_PERCENT { get; set; }
            public string PERFORMANCE_STATUS { get; set; } = string.Empty;
            public string STATUS_COLOR { get; set; } = string.Empty;
        }


        public class RegionSummary
        {
            public string REGION_NAME { get; set; } = string.Empty;
            public int TOTAL_BRANCHES { get; set; }
            public decimal TOTAL_DEPOSIT { get; set; }
            public decimal TOTAL_LOAN { get; set; }
            public decimal TOTAL_RECOVERY { get; set; }
            public decimal AVG_PERFORMANCE_PERCENT { get; set; }
        }


        public class TopBranch
        {
            public int BRANCH_RANK { get; set; }
            public string PBRCODE { get; set; } = string.Empty;
            public string BRANCH_NAME { get; set; } = string.Empty;
            public string BRANCH_MANAGER { get; set; } = string.Empty;
            public decimal OVERALL_ACHIEVEMENT_PERCENT { get; set; }
            public string PERFORMANCE_STATUS { get; set; } = string.Empty;
            public decimal NPA_PERCENT { get; set; }
        }


        public class BranchPerformanceDashboardResponse
        {
            public BranchPerformanceSummary Summary { get; set; } = new();
            public List<BranchPerformanceGrid> BranchGrid { get; set; } = new();
            public List<RegionSummary> RegionSummary { get; set; } = new();
            public List<TopBranch> TopBranches { get; set; } = new();
        }


        public class BranchPerformanceRequest
        {
            public DateTime? TARGET_DATE { get; set; }
            public string? REGION_NAME { get; set; }
            public string? PERFORMANCE_STATUS { get; set; }
        }
    }
}
