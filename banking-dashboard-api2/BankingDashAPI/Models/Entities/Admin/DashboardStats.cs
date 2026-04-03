namespace BankingDashAPI.Models.Entities.Admin
{
    public class DashboardStats
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalRoles { get; set; }
        public int TotalPages { get; set; }
        public int TodayLogins { get; set; }
        public int FailedLogins { get; set; }
        public int ActiveSessions { get; set; } 
    }
}
