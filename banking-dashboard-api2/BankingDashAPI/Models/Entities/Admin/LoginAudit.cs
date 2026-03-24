namespace BankingDashAPI.Models.Entities.Admin
{
    public class LoginAudit
    {
        public long LoginID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string LoginStatus { get; set; } = string.Empty;
        public int FailedAttemptCount { get; set; }
    }
}
