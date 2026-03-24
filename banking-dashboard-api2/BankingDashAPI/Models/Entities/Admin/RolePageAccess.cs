namespace BankingDashAPI.Models.Entities.Admin
{
    public class RolePageAccess
    {
        public int AccessID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int PageID { get; set; }
        public string PageName { get; set; } = string.Empty;
        public bool CanView { get; set; }
        public bool CanExport { get; set; }
        public bool CanDrillDown { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
