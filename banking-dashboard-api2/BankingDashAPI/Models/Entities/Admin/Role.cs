namespace BankingDashAPI.Models.Entities.Admin
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int RoleLevel { get; set; }
        public string RoleDescription { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int UserCount { get; set; } // For dashboard stats
    }
}
