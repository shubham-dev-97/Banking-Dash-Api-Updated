namespace BankingDashAPI.Models.Auth
{
    public class LoginResponse
    {


        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public DateTime? Expiration { get; set; }
        public UserInfo? User { get; set; }
    }

        public class UserInfo
        {
            public int UserID { get; set; }
            public string UserLoginID { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string EmailID { get; set; } = string.Empty;
            public string RoleName { get; set; } = string.Empty;
            public int RoleID { get; set; }
            public int? BranchID { get; set; }
            public int? RegionID { get; set; }
            public string Department { get; set; } = string.Empty;
        }
    
}
