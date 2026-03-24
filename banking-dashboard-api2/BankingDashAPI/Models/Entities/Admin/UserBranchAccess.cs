namespace BankingDashAPI.Models.Entities.Admin
{
    public class UserBranchAccess
    {
        public int AccessID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string AccessLevel { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
