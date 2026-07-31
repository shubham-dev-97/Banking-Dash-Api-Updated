using BankingDashAPI.Models.DTOs;
using BankingDashAPI.Models.Entities;
using BankingDashAPI.Models.Filters;

namespace BankingDashAPI.Services.Interfaces;

public interface IDashboardService
{
    

    Task<List<CustomerCountByCategory>> GetCustomerCountByCategory(CustomerCountFilter filter);


    Task<HomeCustomerSummary> GetHomeCustomerSummary(DateTime? asOnDate);

    List<DateTime> GetAvailableDates();

    DepositOpeningSummary GetDepositOpeningSummary(DateTime asOnDate);

    NPASummary GetNPASummary(DateTime asOnDate);

    List<HCDistribution> GetHCDistribution(DateTime asOnDate);

    List<CASASummary> GetCASASummary(DateTime asOnDate);

    GLDashboardSummary GetGLDashboardSummary(DateTime asOnDate);


    Task<PortfolioOverview> GetPortfolioOverviewAsync(DateTime asOnDate);
    Task<InterestAndOverdueKPI> GetInterestAndOverdueKPIAsync(DateTime asOnDate);

    Task<DepositPortfolioOverview> GetDepositPortfolioOverviewAsync(DateTime asOnDate);

    Task<LoanPortfolioOverview> GetLoanPortfolioOverviewAsync(DateTime asOnDate);

    Task<List<DepositTrend>> GetDepositTrendLast6MonthsAsync(DateTime asOnDate);
    Task<List<LoanTrend>> GetLoanTrendLast6MonthsAsync(DateTime asOnDate, CancellationToken cancellationToken = default);


    Task<List<AlmBucketRBI>> GetAlmBucketRBIAsync(DateTime asOnDate);


    Task<List<DepLoanMonthlyTrend>> GetDepLoanMonthlyTrendWithCDRatioAsync(DateTime asOnDate);

    Task<List<RbiLoanAuditDump>> GetRbiLoanAuditDumpAsync(DateTime asOnDate);

    Task<List<RbiDepositAuditDump>> GetRbiDepositAuditDumpAsync(DateTime asOnDate);


    Task<(List<RbiDepositAuditDump> Data, int TotalCount)> GetRbiDepositAuditDumpPaginatedAsync(DateTime asOnDate, int pageNumber, int pageSize);
}
