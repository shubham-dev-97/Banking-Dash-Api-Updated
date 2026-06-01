using BankingDashAPI.Models.Entities;
using System.Threading.Tasks;
using static BankingDashAPI.Models.Entities.BankKPIDashboard;

namespace BankingDashAPI.Services.Interfaces
{
    public interface IBankKPIService
    {
        Task<BankKPIDashboardResponse> GetBankKPIDashboardAsync(BankKPIRequest request);
    }
}