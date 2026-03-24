namespace BankingDashAPI.Models.Entities.Admin
{
    public class UserRegionAccess
    {
        public int AccessID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int RegionID { get; set; }
        public string RegionName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
