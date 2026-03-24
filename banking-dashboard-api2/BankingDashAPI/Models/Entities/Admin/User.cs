namespace BankingDashAPI.Models.Entities.Admin
{
    public class User
    {
        public int UserID { get; set; }
        public string UserLoginID { get; set; } = string.Empty;
        public string EmployeeID { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string EmailID { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int? BranchID { get; set; }
        public int? RegionID { get; set; }
        public string Department { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime? LastLoginDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
