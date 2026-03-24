using System.Collections.Generic;

namespace BankingDashAPI.Models.DTOs.Admin

{
    public class BranchAccessRequest
    {
        public List<int> BranchIds { get; set; } = new List<int>();
        public string AccessLevel { get; set; } = string.Empty;
    }
}
