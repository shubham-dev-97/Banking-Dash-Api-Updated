namespace BankingDashAPI.Models.Entities.Admin
{
    public class Page
    {
        public int PageID { get; set; }
        public string PageName { get; set; } = string.Empty;
        public string PageCode { get; set; } = string.Empty;
        public string PageCategory { get; set; } = string.Empty;
        public string PageURL { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
