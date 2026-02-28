using BankingDashAPI.Models.DTOs;
using BankingDashAPI.Models.Entities;
using BankingDashAPI.Models.Filters;

namespace BankingDashAPI.Services.Interfaces;

public interface IDashboardService
{
    Task<HomeKpi?> GetHomeKpi(HomeFilter? filter);

    Task<List<DepositAnalysis>> GetDepositAnalysis(DepositFilter? filter);

    Task<List<LoanAnalysis>> GetLoanAnalysis(LoanFilter? filter);

    Task<List<MonthlyTrend>> GetMonthlyTrend(MonthlyTrendFilter? filter);

    Task<List<BankingSummary>> GetBankingSummary(SummaryFilter? filter);


    Task<List<CustomerCountByCategory>> GetCustomerCountByCategory(CustomerCountFilter filter);


    Task<HomeCustomerSummary> GetHomeCustomerSummary(DateTime? asOnDate);

    List<DateTime> GetAvailableDates();

    DepositOpeningSummary GetDepositOpeningSummary(DateTime asOnDate);
}   