using System.ComponentModel.DataAnnotations;

namespace BankingDashAPI.Models.Auth
{
    public class LoginRequest
    {
        [Required]
        public string UserLoginID { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
