using BankingDashAPI.Models.Auth;
using System.Threading.Tasks;

namespace BankingDashAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> AuthenticateAsync(LoginRequest request);
        string GenerateJwtToken(UserInfo user);
    }
}