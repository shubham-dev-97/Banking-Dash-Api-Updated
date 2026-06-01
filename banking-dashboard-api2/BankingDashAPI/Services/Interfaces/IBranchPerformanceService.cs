using static BankingDashAPI.Models.Entities.BranchPerformanceDashboard;

namespace BankingDashAPI.Services.Interfaces
{
    public interface IBranchPerformanceService
    {

        Task<BranchPerformanceDashboardResponse> GetBranchPerformanceDashboardAsync(BranchPerformanceRequest request);
    }
}
