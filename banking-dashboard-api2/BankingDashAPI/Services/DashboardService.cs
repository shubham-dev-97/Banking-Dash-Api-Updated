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


    // To Get the NPA Summary
    public NPASummary GetNPASummary(DateTime asOnDate)
    {
        try
        {
            _logger.LogInformation("Getting NPA summary for date: {Date}", asOnDate.ToString("yyyy-MM-dd"));

            var result = new NPASummary();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand("sp_NPASummary", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AsOnDate", asOnDate.Date);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.TotalNPAOpenLast30Days = reader.GetInt32(0);
                            result.TotalNPAAccountInBank = reader.GetInt32(1);
                            result.TotalNPAAmount = reader.GetDecimal(2);
                            result.OpeningPercentage = reader.GetDecimal(3);
                        }
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting NPA summary");
            return new NPASummary();
        }
    }



    // To Get the HC Distribution
    public List<HCDistribution> GetHCDistribution(DateTime asOnDate)
    {
        try
        {
            _logger.LogInformation("Getting HC distribution for date: {Date}", asOnDate.ToString("yyyy-MM-dd"));

            var result = new List<HCDistribution>();

            var query = @"
            SELECT COUNT(*) as count, HC 
            FROM BANK_LOAN_DATA_DUMP  
            WHERE P_AS_ON_DATE = @AsOnDate
            AND DON IS NOT NULL 
            AND ACCOUNT_STATUS='O'
            GROUP BY HC
            ORDER BY count DESC";

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AsOnDate", asOnDate.Date);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new HCDistribution
                            {
                                Count = reader.GetInt32(0),
                                HC = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            _logger.LogInformation("Retrieved {Count} HC categories", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting HC distribution");
            return new List<HCDistribution>();
        }
    }


    // CasaSummary
    public List<CASASummary> GetCASASummary(DateTime asOnDate)
    {
        try
        {
            _logger.LogInformation("Getting CASA summary for date: {Date}", asOnDate.ToString("yyyy-MM-dd"));

            var result = new List<CASASummary>();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand("SP_DEPOSIT_SUMMARY_CASA", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AS_ON_DATE", asOnDate.Date);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new CASASummary
                            {
                                Deposit_Type = reader.GetString(0),
                                Total_Balance = reader.GetDecimal(1),
                                Cnt = reader.GetInt32(2)
                            });
                        }
                    }
                }
            }

            _logger.LogInformation("Retrieved {Count} CASA summary rows", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting CASA summary");
            return new List<CASASummary>();
        }
    }


    // Gl Summary
    public GLDashboardSummary GetGLDashboardSummary(DateTime asOnDate)
    {
        try
        {
            _logger.LogInformation("Getting GL Dashboard summary for date: {Date}", asOnDate.ToString("yyyy-MM-dd"));

            var result = new GLDashboardSummary();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand("SP_GL_DASHBOARD_SUMMARY", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AS_ON_DATE", asOnDate.Date);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.Total_Assets = reader.GetDecimal(0);
                            result.Total_Liabilities = reader.GetDecimal(1);
                            result.Total_Income = reader.GetDecimal(2);
                            result.Total_Expense = reader.GetDecimal(3);
                            result.Total_Debit = reader.GetDecimal(4);
                            result.Total_Credit = reader.GetDecimal(5);
                            result.Net_Profit = reader.GetDecimal(6);
                            result.Net_Position = reader.GetDecimal(7);
                        }
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting GL Dashboard summary");
            return new GLDashboardSummary();
        }
    }
}