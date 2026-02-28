using BankingDashAPI.Data;
using BankingDashAPI.Models.Entities;
using BankingDashAPI.Models.Filters;
using BankingDashAPI.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using BankingDashAPI.Models.DTOs;

namespace BankingDashAPI.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DashboardService> _logger;


    public DashboardService(AppDbContext context, ILogger<DashboardService> logger)
    {
        _context = context;
        _logger = logger;
    }


    // HOME KPI
    public async Task<HomeKpi?> GetHomeKpi(HomeFilter? filter)
    {
        var query = _context.HomeKpi.AsQueryable();
        return await query.AsNoTracking().FirstOrDefaultAsync();
    }



    // DEPOSIT ANALYSIS
    public async Task<List<DepositAnalysis>> GetDepositAnalysis(DepositFilter? filter)
    {
        var query = _context.DepositAnalysis.AsQueryable();

        if (filter != null)
        {
            if (!string.IsNullOrEmpty(filter.BranchCode))
                query = query.Where(x => x.BranchCode == filter.BranchCode);  

            if (!string.IsNullOrEmpty(filter.ProductCode))
                query = query.Where(x => x.ProductCode == filter.ProductCode);  

            if (!string.IsNullOrEmpty(filter.SchemeCode))
                query = query.Where(x => x.SchemeCode == filter.SchemeCode); 

            if (!string.IsNullOrEmpty(filter.CustomerCategory))
                query = query.Where(x => x.CustomerCategory == filter.CustomerCategory);  

            if (!string.IsNullOrEmpty(filter.AccountStatus))
                query = query.Where(x => x.AccountStatus == filter.AccountStatus);  

            if (!string.IsNullOrEmpty(filter.Gender))
                query = query.Where(x => x.CustomerGender == filter.Gender);  

            if (filter.Year.HasValue)
                query = query.Where(x => x.OpenYear == filter.Year);

            if (filter.Month.HasValue)
                query = query.Where(x => x.OpenMonth == filter.Month);
        }

        return await query.AsNoTracking().ToListAsync();
    }


    // LOAN ANALYSIS
    public async Task<List<LoanAnalysis>> GetLoanAnalysis(LoanFilter? filter)
    {
        var query = _context.LoanAnalysis.AsQueryable();

        if (filter != null)
        {
            if (!string.IsNullOrEmpty(filter.BranchCode))
                query = query.Where(x => x.BranchCode == filter.BranchCode);  
            if (!string.IsNullOrEmpty(filter.SchemeCode))
                query = query.Where(x => x.LoanSchemeCode == filter.SchemeCode); 

            if (!string.IsNullOrEmpty(filter.Purpose))
                query = query.Where(x => x.Purpose == filter.Purpose);  
            if (!string.IsNullOrEmpty(filter.Segment))
                query = query.Where(x => x.Segment == filter.Segment);  

            if (!string.IsNullOrEmpty(filter.PrioritySector))
                query = query.Where(x => x.PrioritySector == filter.PrioritySector);  

            if (!string.IsNullOrEmpty(filter.SecureType))
                query = query.Where(x => x.SecureUnsecure == filter.SecureType); 

            if (!string.IsNullOrEmpty(filter.AccountStatus))
                query = query.Where(x => x.AccountStatus == filter.AccountStatus);  

            if (filter.Year.HasValue)
                query = query.Where(x => x.DisbursementYear == filter.Year);

            if (filter.Month.HasValue)
                query = query.Where(x => x.DisbursementMonth == filter.Month);
        }

        return await query.AsNoTracking().ToListAsync();
    }


    // MONTHLY TREND
    public async Task<List<MonthlyTrend>> GetMonthlyTrend(MonthlyTrendFilter? filter)
    {
        var query = _context.MonthlyTrend.AsQueryable();

        if (filter != null)
        {
            if (filter.Year.HasValue)
                query = query.Where(x => x.Year == filter.Year); 

            if (filter.Month.HasValue)
                query = query.Where(x => x.Month == filter.Month); 
        }

        return await query.AsNoTracking().ToListAsync();
    }



    // BANKING SUMMARY
    public async Task<List<BankingSummary>> GetBankingSummary(SummaryFilter? filter)
    {
        var query = _context.BankingSummary.AsQueryable();

        if (filter != null)
        {
            if (!string.IsNullOrEmpty(filter.BranchCode))
                query = query.Where(x => x.BranchCode == filter.BranchCode);  
        }

        return await query.AsNoTracking().ToListAsync();
    }


    public async Task<List<CustomerCountByCategory>> GetCustomerCountByCategory(CustomerCountFilter filter)
    {
        try
        {
            var result = new List<CustomerCountByCategory>();

            // Get the connection string from DbContext
            var connectionString = _context.Database.GetConnectionString();

            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("sp_GetCustomerCountByCategory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AsOnDate", filter.AsOnDate.Date);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new CustomerCountByCategory
                            {
                                TotalCustomer = reader.GetInt32(0), // First column
                                Cat = reader.GetString(1)          // Second column
                            });
                        }
                    }
                }
            }

          
          
            Console.WriteLine($"Date: {filter.AsOnDate:yyyy-MM-dd}");
            Console.WriteLine($"Returned {result.Count} rows:");
            foreach (var item in result)
            {
                Console.WriteLine($"  {item.Cat}: {item.TotalCustomer}");
            }
           

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw; // Throw so we can see the error in API response
        }
    }


    //To Get the Customer Summary
    public async Task<HomeCustomerSummary> GetHomeCustomerSummary(DateTime? asOnDate)
    {
        try
        {
            DateTime targetDate = asOnDate ?? DateTime.Now.Date;

            _logger.LogInformation("Fetching home customer summary for date: {Date}", targetDate.ToString("yyyy-MM-dd"));

            // Make sure this method name matches exactly
            var customerData = await GetCustomerCountByCategory(new CustomerCountFilter { AsOnDate = targetDate });

            _logger.LogInformation("Got {Count} customer categories", customerData?.Count ?? 0);

            var depositCustomers = customerData?.FirstOrDefault(x => x.Cat == "depo")?.TotalCustomer ?? 0;
            var loanCustomers = customerData?.FirstOrDefault(x => x.Cat == "loan")?.TotalCustomer ?? 0;
            var npaCustomers = customerData?.FirstOrDefault(x => x.Cat == "NPA")?.TotalCustomer ?? 0;

            var summary = new HomeCustomerSummary
            {
                TotalDepositCustomers = depositCustomers,
                TotalLoanCustomers = loanCustomers,
                TotalCustomers = depositCustomers + loanCustomers,
                NpaCustomers = npaCustomers
            };

            _logger.LogInformation("Home customer summary - Total: {Total}, Deposit: {Deposit}, Loan: {Loan}, NPA: {NPA}",
                summary.TotalCustomers, summary.TotalDepositCustomers, summary.TotalLoanCustomers, summary.NpaCustomers);

            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetHomeCustomerSummaryAsync for date: {Date}", asOnDate);
            throw; // Re-throw so controller can catch it
        }
    }


    //To Get the Available Dates

    public List<DateTime> GetAvailableDates()
    {
        try
        {
            _logger.LogInformation("Getting available dates from stored procedure");

            var dates = new List<DateTime>();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand("sp_GetAvailableDates", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dates.Add(reader.GetDateTime(0)); // First column is the date
                        }
                    }
                }
            }

            _logger.LogInformation("Retrieved {Count} dates", dates.Count);
            return dates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available dates");
            return new List<DateTime>(); // Return empty list on error
        }
    }



    // To Get the Deosit Opening Summary
    public DepositOpeningSummary GetDepositOpeningSummary(DateTime asOnDate)
    {
        try
        {
            _logger.LogInformation("Getting deposit opening summary for date: {Date}", asOnDate.ToString("yyyy-MM-dd"));

            var result = new DepositOpeningSummary();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand("sp_DepositOpeningSummary", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AsOnDate", asOnDate.Date);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.TotalDepositOpenLast30Days = reader.GetInt32(0);
                            result.TotalDepositAccountInBank = reader.GetInt32(1);
                            result.TotalDepositAmount = reader.GetDecimal(2);
                            result.OpeningPercentage = reader.GetDecimal(3);
                        }
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting deposit opening summary");
            return new DepositOpeningSummary();
        }
    }
}