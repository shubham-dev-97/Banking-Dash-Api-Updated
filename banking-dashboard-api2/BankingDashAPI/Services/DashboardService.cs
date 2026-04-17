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


    public async Task<PortfolioOverview> GetPortfolioOverviewAsync(DateTime asOnDate)
    {
        try
        {
            _logger.LogInformation("Fetching portfolio overview for date: {Date}", asOnDate.ToString("yyyy-MM-dd"));

            var result = new PortfolioOverview();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand("sp_PortfolioOverview", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AsOnDate", asOnDate.Date);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            result.Total_Deposit = reader.GetDecimal(0);
                            result.Total_Loan = reader.GetDecimal(1);
                            result.Net_Position = reader.GetDecimal(2);
                            result.Loan_To_Deposit_Ratio = reader.GetDecimal(3);
                        }
                    }
                }
            }

            _logger.LogInformation("Portfolio overview retrieved successfully");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching portfolio overview for date: {Date}", asOnDate);
            throw;
        }
    }

    public async Task<InterestAndOverdueKPI> GetInterestAndOverdueKPIAsync(DateTime asOnDate)
    {
        try
        {
            _logger.LogInformation("Fetching interest and overdue KPI for date: {Date}", asOnDate.ToString("yyyy-MM-dd"));

            var result = new InterestAndOverdueKPI();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand("sp_Interest_And_Overdue_KPI", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AsOnDate", asOnDate.Date);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            result.Avg_Loan_Interest_Rate = reader.GetDecimal(0);
                            result.Avg_Deposit_Interest_Rate = reader.GetDecimal(1);
                            result.Overdue_Amount = reader.GetDecimal(2);
                            result.Avg_Account_Size = reader.GetDecimal(3);
                        }
                    }
                }
            }

            _logger.LogInformation("Interest and overdue KPI retrieved successfully");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching interest and overdue KPI for date: {Date}", asOnDate);
            throw;
        }
    }


    public async Task<DepositPortfolioOverview> GetDepositPortfolioOverviewAsync(DateTime asOnDate)
    {
        try
        {
            _logger.LogInformation("Fetching deposit portfolio overview for date: {Date}", asOnDate.ToString("yyyy-MM-dd"));

            var result = new DepositPortfolioOverview();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand("sp_DepositPortfolioOverview", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AsOnDate", asOnDate.Date);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            result.Total_Balance = reader.GetDecimal(0);
                            result.Total_Accounts = reader.GetInt32(1);
                            result.Avg_Balance = reader.GetDecimal(2);
                            result.Avg_Interest_Rate = reader.GetDecimal(3);
                            result.Active_Accounts = reader.GetInt32(4);
                            result.Dormant_Accounts = reader.GetInt32(5);
                            result.Closed_Accounts = reader.GetInt32(6);
                            result.Avg_Account_Size = reader.GetDecimal(7);
                        }
                    }
                }
            }

            _logger.LogInformation("Deposit portfolio overview retrieved successfully");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching deposit portfolio overview for date: {Date}", asOnDate);
            throw;
        }
    }


    public async Task<LoanPortfolioOverview> GetLoanPortfolioOverviewAsync(DateTime asOnDate)
    {
        try
        {
            _logger.LogInformation("Fetching loan portfolio overview for date: {Date}", asOnDate.ToString("yyyy-MM-dd"));

            var result = new LoanPortfolioOverview();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand("sp_LoanPortfolioOverview", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AsOnDate", asOnDate.Date);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            result.Total_Loan_Amount = reader.GetDecimal(0);
                            result.Total_Outstanding = reader.GetDecimal(1);
                            result.Total_Overdue = reader.GetDecimal(2);
                            result.Avg_Interest_Rate = reader.GetDecimal(3);
                            result.Total_Accounts = reader.GetInt32(4);
                            result.Active_Accounts = reader.GetInt32(5);
                            result.Overdue_Accounts = reader.GetInt32(6);
                            result.Avg_Loan_Size = reader.GetDecimal(7);
                        }
                    }
                }
            }

            _logger.LogInformation("Loan portfolio overview retrieved successfully");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching loan portfolio overview for date: {Date}", asOnDate);
            throw;
        }
    }


    public async Task<List<DepositTrend>> GetDepositTrendLast6MonthsAsync(DateTime asOnDate)
    {
        try
        {
            _logger.LogInformation("Fetching deposit trend for last 6 months from date: {Date}",
                asOnDate.ToString("yyyy-MM-dd"));

            var trends = new List<DepositTrend>();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand("sp_GetDepositTrendLast6Months", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AsOnDate", asOnDate.Date);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            trends.Add(new DepositTrend
                            {
                                Year = reader.GetInt32(0),
                                Month = reader.GetInt32(1),
                                MonthName = reader.GetString(2),
                                TotalBalance = reader.GetDecimal(3),
                                AccountCount = reader.GetInt32(4),
                                AverageBalance = reader.GetDecimal(5)
                            });
                        }
                    }
                }
            }

            _logger.LogInformation("Retrieved {Count} months of deposit trend data", trends.Count);
            return trends;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching deposit trend data");
            return new List<DepositTrend>();
        }
    }

    public async Task<List<LoanTrend>> GetLoanTrendLast6MonthsAsync(DateTime asOnDate)
    {
        try
        {
            _logger.LogInformation("Fetching loan trend for last 6 months from date: {Date}",
                asOnDate.ToString("yyyy-MM-dd"));

            var trends = new List<LoanTrend>();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand("sp_GetLoanTrendLast6Months", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AsOnDate", asOnDate.Date);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            trends.Add(new LoanTrend
                            {
                                Year = reader.GetInt32(0),
                                Month = reader.GetInt32(1),
                                MonthName = reader.GetString(2),
                                TotalOutstanding = reader.GetDecimal(3),
                                TotalSanctioned = reader.GetDecimal(4),
                                AccountCount = reader.GetInt32(5),
                                AverageLoanSize = reader.GetDecimal(6)
                            });
                        }
                    }
                }
            }

            _logger.LogInformation("Retrieved {Count} months of loan trend data", trends.Count);
            return trends;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching loan trend data");
            return new List<LoanTrend>();
        }
    }


    public async Task<List<AlmBucketRBI>> GetAlmBucketRBIAsync(DateTime asOnDate)
    {
        try
        {
            _logger.LogInformation("Fetching ALM Bucket RBI data for date: {Date}",
                asOnDate.ToString("yyyy-MM-dd"));

            var data = new List<AlmBucketRBI>();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand("SP_ALM_BUCKET_RBI", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ASONDATE", asOnDate.Date);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            data.Add(new AlmBucketRBI
                            {
                                RBI_BUCKET = reader.GetString(0),
                                NO_OF_ACCOUNTS = reader.GetInt32(1),
                                OUTSTANDING_BALANCE = reader.GetDecimal(2),
                                MATURITY_AMOUNT = reader.GetDecimal(3)
                            });
                        }
                    }
                }
            }

            _logger.LogInformation("Retrieved {Count} ALM buckets", data.Count);
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching ALM Bucket RBI data");
            throw;
        }
    }

    public async Task<List<DepLoanMonthlyTrend>> GetDepLoanMonthlyTrendWithCDRatioAsync(DateTime asOnDate)
    {
        try
        {
            _logger.LogInformation("Fetching Deposit vs Loan monthly trend with CD Ratio for date: {Date}",
                asOnDate.ToString("yyyy-MM-dd"));

            var trends = new List<DepLoanMonthlyTrend>();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand("SP_DEP_LOAN_MONTHEND_WITH_CDRATIO", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AsOnDate", asOnDate.Date);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            trends.Add(new DepLoanMonthlyTrend
                            {
                                MONTH_END = reader.GetDateTime(0),
                                DepositBal = reader.GetDecimal(1),
                                LoanBal = reader.GetDecimal(2),
                                CD_RATIO_PERCENT = reader.GetDecimal(3),
                                Deposit_tag = reader.GetString(4),
                                Loan_tag = reader.GetString(5)
                            });
                        }
                    }
                }
            }

            _logger.LogInformation("Retrieved {Count} months of deposit-loan trend data", trends.Count);
            return trends;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching deposit-loan monthly trend data");
            return new List<DepLoanMonthlyTrend>();
        }


    }


    public async Task<List<RbiLoanAuditDump>> GetRbiLoanAuditDumpAsync(DateTime asOnDate)
    {
        try
        {
            _logger.LogInformation("Fetching RBI Loan Audit Dump for date: {Date}",
                asOnDate.ToString("yyyy-MM-dd"));

            var data = new List<RbiLoanAuditDump>();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand("SP_RBI_LOAN_AUDIT_DUMP", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ASONDATE", asOnDate.Date);
                    command.CommandTimeout = 300; // 5 minutes timeout

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var record = new RbiLoanAuditDump();

                            // Safe reading with Convert for all columns - using lowercase/mixed-case properties
                            record.reporT_DATE = reader.GetDateTime(0);
                            record.brancH_ID = Convert.ToInt64(reader.GetValue(1));
                            record.loaN_ACCOUNT_NO = Convert.ToInt64(reader.GetValue(2));
                            record.customeR_ID = reader.GetString(3);
                            record.borroweR_NAME = reader.GetString(4);
                            record.customeR_TYPE = reader.GetString(5);
                            record.dob = reader.IsDBNull(6) ? null : reader.GetDateTime(6);
                            record.gender = reader.GetString(7);
                            record.paN_NO = reader.GetString(8);
                            record.ckyC_ID = reader.GetString(9);
                            record.gsT_NO = reader.GetString(10);
                            record.addresS_LINE1 = reader.GetString(11);
                            record.addresS_LINE2 = reader.GetString(12);
                            record.addresS_LINE3 = reader.GetString(13);
                            record.city = reader.GetString(14);
                            record.pincode = reader.GetString(15);
                            record.mobilE_NO = reader.GetString(16);
                            record.emaiL_ID = reader.GetString(17);
                            record.loaN_PRODUCT_CODE = reader.GetString(18);
                            record.loaN_PURPOSE = reader.GetString(19);
                            record.prioritY_SECTOR = reader.GetString(20);
                            record.psL_CODE = reader.GetString(21);
                            record.customeR_SEGMENT = reader.GetString(22);
                            record.sanctioN_DATE = reader.IsDBNull(23) ? null : reader.GetDateTime(23);
                            record.disbursemenT_DATE = reader.IsDBNull(24) ? null : reader.GetDateTime(24);
                            record.sanctioN_AMOUNT = Convert.ToDecimal(reader.GetValue(25));
                            record.sanctioneD_BY = reader.GetString(26);
                            record.accounT_OPEN_DATE = reader.IsDBNull(27) ? null : reader.GetDateTime(27);
                            record.maturitY_DATE = reader.IsDBNull(28) ? null : reader.GetDateTime(28);
                            record.outstandinG_AMOUNT = Convert.ToDecimal(reader.GetValue(29));
                            record.interesT_RECEIVABLE = Convert.ToDecimal(reader.GetValue(30));
                            record.emI_AMOUNT = Convert.ToDecimal(reader.GetValue(31));
                            record.securitY_VALUE = Convert.ToDecimal(reader.GetValue(32));
                            record.roi = Convert.ToDecimal(reader.GetValue(33));
                            record.repaymenT_MODE = reader.GetString(34);
                            record.tenure = Convert.ToDecimal(reader.GetValue(35));
                            record.asseT_CLASSIFICATION = reader.GetString(36);
                            record.internaL_RATING = reader.GetString(37);
                            record.weakeR_SECTION_FLAG = reader.GetString(38);
                            record.securitY_TYPE = reader.GetString(39);
                            record.overduE_AMOUNT = Convert.ToDecimal(reader.GetValue(40));
                            record.dayS_PAST_DUE = Convert.ToDecimal(reader.GetValue(41));
                            record.datE_OF_DEFAULT = reader.IsDBNull(42) ? null : reader.GetDateTime(42);
                            record.provisioN_PERCENT = Convert.ToDecimal(reader.GetValue(43));
                            record.secureD_PROVISION = Convert.ToDecimal(reader.GetValue(44));
                            record.unsecureD_PROVISION = Convert.ToDecimal(reader.GetValue(45));
                            record.totaL_PROVISION = Convert.ToDecimal(reader.GetValue(46));
                            record.rbI_ASSET_CLASS = reader.GetString(47);
                            record.reporT_TYPE = reader.GetString(48);

                            data.Add(record);
                        }
                    }
                }
            }

            _logger.LogInformation("Retrieved {Count} loan audit records", data.Count);
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching RBI Loan Audit Dump data");
            throw;
        }
    }

    public async Task<List<RbiDepositAuditDump>> GetRbiDepositAuditDumpAsync(DateTime asOnDate)
    {
        try
        {
            _logger.LogInformation("Fetching RBI Deposit Audit Dump for date: {Date}",
                asOnDate.ToString("yyyy-MM-dd"));

            var data = new List<RbiDepositAuditDump>();

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand("SP_RBI_DEPOSIT_AUDIT_DUMP", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AS_ON_DATE", asOnDate.Date);
                    command.CommandTimeout = 300;

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var record = new RbiDepositAuditDump();

                            // Map all columns here (same as before)
                            record.REPORT_DATE = reader.GetDateTime(0);
                            record.BRANCH_ID = reader.GetString(1);
                            record.GL_CODE = reader.GetString(2);
                            record.ACCOUNT_NO = reader.GetString(3);
                            record.ACCOUNT_NAME = reader.GetString(4);
                            record.SCHEME_CODE = reader.GetString(5);
                            record.PRODUCT_TYPE = reader.GetString(6);
                            record.CUSTOMER_ID = reader.GetString(7);
                            record.CUSTOMER_NAME = reader.GetString(8);
                            record.FATHER_NAME = reader.GetString(9);
                            record.MOTHER_NAME = reader.GetString(10);
                            record.DOB = reader.IsDBNull(11) ? null : reader.GetDateTime(11);
                            record.GENDER = reader.GetString(12);
                            record.CUSTOMER_CATEGORY = reader.GetString(13);
                            record.CUSTOMER_CLASSIFICATION = reader.GetString(14);
                            record.CUSTOMER_STATUS = reader.GetString(15);
                            record.KYC_STATUS = reader.GetString(16);
                            record.CKYC_NUMBER = reader.GetString(17);
                            record.PAN = reader.GetString(18);
                            record.FATCA_STATUS = reader.GetString(19);
                            record.AML_RISK_CATEGORY = reader.GetString(20);
                            record.FORM15G_H_STATUS = reader.GetString(21);
                            record.ACCOUNT_STATUS = reader.GetString(22);
                            record.INOPERATIVE_FLAG = reader.GetString(23);
                            record.DORMANT_STATUS = reader.GetString(24);
                            record.LIEN_STATUS = reader.GetString(25);
                            record.ACCOUNT_OPEN_DATE = reader.IsDBNull(26) ? null : reader.GetDateTime(26);
                            record.ACCOUNT_CLOSE_DATE = reader.IsDBNull(27) ? null : reader.GetDateTime(27);
                            record.LAST_TRANSACTION_DATE = reader.IsDBNull(28) ? null : reader.GetDateTime(28);
                            record.DEPOSIT_START_DATE = reader.IsDBNull(29) ? null : reader.GetDateTime(29);
                            record.MATURITY_DATE = reader.IsDBNull(30) ? null : reader.GetDateTime(30);
                            record.DEPOSIT_END_DATE = reader.IsDBNull(31) ? null : reader.GetDateTime(31);
                            record.CURRENT_BALANCE = reader.GetDecimal(32);
                            record.DEPOSIT_AMOUNT = reader.GetDecimal(33);
                            record.MATURITY_AMOUNT = reader.GetDecimal(34);
                            record.TOTAL_CREDITS = reader.GetDecimal(35);
                            record.TOTAL_DEBITS = reader.GetDecimal(36);
                            record.AVG_QUARTERLY_BALANCE = reader.GetDecimal(37);
                            record.INTEREST_RATE = reader.GetDecimal(38);
                            record.INTEREST_PAYOUT_MODE = reader.GetString(39);
                            record.TOTAL_TRANSACTIONS = Convert.ToInt32(reader.GetDecimal(40));
                            record.ADDRESS_LINE1 = reader.GetString(41);
                            record.ADDRESS_LINE2 = reader.GetString(42);
                            record.ADDRESS_LINE3 = reader.GetString(43);
                            record.CITY = reader.GetString(44);
                            record.PIN_CODE = reader.GetString(45);
                            record.STATE_CODE = reader.GetString(46);
                            record.MOBILE_NO = reader.GetString(47);
                            record.EMAIL_ID = reader.GetString(48);
                            record.MEMBER_TYPE = reader.GetString(49);
                            record.MEMBER_ID = reader.GetString(50);
                            record.DEPOSIT_RECEIPT_NO = reader.GetString(51);
                            record.DEPOSIT_TYPE_CODE = reader.GetString(52);
                            record.DEPOSIT_STATUS = reader.GetString(53);
                            record.DEPOSIT_TENURE = reader.GetString(54);
                            record.DEPOSIT_TYPE = reader.GetString(55);
                            record.DEPOSIT_SIZE_FLAG = reader.GetString(56);
                            record.KYC_RISK_FLAG = reader.GetString(57);
                            record.AML_RISK_LEVEL = reader.GetString(58);
                            record.ACCOUNT_ACTIVITY_STATUS = reader.GetString(59);
                            record.AUDIT_REMARK = reader.GetString(60);

                            data.Add(record);
                        }
                    }
                }
            }

            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching RBI Deposit Audit Dump data");
            throw;
        }
    }




    public async Task<(List<RbiDepositAuditDump> Data, int TotalCount)> GetRbiDepositAuditDumpPaginatedAsync(DateTime asOnDate, int pageNumber, int pageSize)
    {
        try
        {
            _logger.LogInformation("Fetching RBI Deposit Audit Dump for date: {Date}, Page: {Page}, Size: {Size}",
                asOnDate.ToString("yyyy-MM-dd"), pageNumber, pageSize);

            var data = new List<RbiDepositAuditDump>();
            int totalCount = 0;

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (var command = new SqlCommand("SP_RBI_DEPOSIT_AUDIT_DUMP_PAGINATED", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AS_ON_DATE", asOnDate.Date);
                    command.Parameters.AddWithValue("@PAGE_NUMBER", pageNumber);
                    command.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
                    command.CommandTimeout = 60;

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // First result set - Total Count
                        if (await reader.ReadAsync())
                        {
                            totalCount = reader.GetInt32(0);
                        }

                        // Second result set - Data
                        await reader.NextResultAsync();
                        while (await reader.ReadAsync())
                        {
                            var record = new RbiDepositAuditDump();

                            // Map all columns here (same as above)
                            record.REPORT_DATE = reader.GetDateTime(0);
                            record.BRANCH_ID = reader.GetString(1);
                            record.GL_CODE = reader.GetString(2);
                            record.ACCOUNT_NO = reader.GetString(3);
                            record.ACCOUNT_NAME = reader.GetString(4);
                            record.SCHEME_CODE = reader.GetString(5);
                            record.PRODUCT_TYPE = reader.GetString(6);
                            record.CUSTOMER_ID = reader.GetString(7);
                            record.CUSTOMER_NAME = reader.GetString(8);
                            record.FATHER_NAME = reader.GetString(9);
                            record.MOTHER_NAME = reader.GetString(10);
                            record.DOB = reader.IsDBNull(11) ? null : reader.GetDateTime(11);
                            record.GENDER = reader.GetString(12);
                            record.CUSTOMER_CATEGORY = reader.GetString(13);
                            record.CUSTOMER_CLASSIFICATION = reader.GetString(14);
                            record.CUSTOMER_STATUS = reader.GetString(15);
                            record.KYC_STATUS = reader.GetString(16);
                            record.CKYC_NUMBER = reader.GetString(17);
                            record.PAN = reader.GetString(18);
                            record.FATCA_STATUS = reader.GetString(19);
                            record.AML_RISK_CATEGORY = reader.GetString(20);
                            record.FORM15G_H_STATUS = reader.GetString(21);
                            record.ACCOUNT_STATUS = reader.GetString(22);
                            record.INOPERATIVE_FLAG = reader.GetString(23);
                            record.DORMANT_STATUS = reader.GetString(24);
                            record.LIEN_STATUS = reader.GetString(25);
                            record.ACCOUNT_OPEN_DATE = reader.IsDBNull(26) ? null : reader.GetDateTime(26);
                            record.ACCOUNT_CLOSE_DATE = reader.IsDBNull(27) ? null : reader.GetDateTime(27);
                            record.LAST_TRANSACTION_DATE = reader.IsDBNull(28) ? null : reader.GetDateTime(28);
                            record.DEPOSIT_START_DATE = reader.IsDBNull(29) ? null : reader.GetDateTime(29);
                            record.MATURITY_DATE = reader.IsDBNull(30) ? null : reader.GetDateTime(30);
                            record.DEPOSIT_END_DATE = reader.IsDBNull(31) ? null : reader.GetDateTime(31);
                            record.CURRENT_BALANCE = reader.GetDecimal(32);
                            record.DEPOSIT_AMOUNT = reader.GetDecimal(33);
                            record.MATURITY_AMOUNT = reader.GetDecimal(34);
                            record.TOTAL_CREDITS = reader.GetDecimal(35);
                            record.TOTAL_DEBITS = reader.GetDecimal(36);
                            record.AVG_QUARTERLY_BALANCE = reader.GetDecimal(37);
                            record.INTEREST_RATE = reader.GetDecimal(38);
                            record.INTEREST_PAYOUT_MODE = reader.GetString(39);
                            record.TOTAL_TRANSACTIONS = Convert.ToInt32(reader.GetDecimal(40));
                            record.ADDRESS_LINE1 = reader.GetString(41);
                            record.ADDRESS_LINE2 = reader.GetString(42);
                            record.ADDRESS_LINE3 = reader.GetString(43);
                            record.CITY = reader.GetString(44);
                            record.PIN_CODE = reader.GetString(45);
                            record.STATE_CODE = reader.GetString(46);
                            record.MOBILE_NO = reader.GetString(47);
                            record.EMAIL_ID = reader.GetString(48);
                            record.MEMBER_TYPE = reader.GetString(49);
                            record.MEMBER_ID = reader.GetString(50);
                            record.DEPOSIT_RECEIPT_NO = reader.GetString(51);
                            record.DEPOSIT_TYPE_CODE = reader.GetString(52);
                            record.DEPOSIT_STATUS = reader.GetString(53);
                            record.DEPOSIT_TENURE = reader.GetString(54);
                            record.DEPOSIT_TYPE = reader.GetString(55);
                            record.DEPOSIT_SIZE_FLAG = reader.GetString(56);
                            record.KYC_RISK_FLAG = reader.GetString(57);
                            record.AML_RISK_LEVEL = reader.GetString(58);
                            record.ACCOUNT_ACTIVITY_STATUS = reader.GetString(59);
                            record.AUDIT_REMARK = reader.GetString(60);

                            data.Add(record);
                        }
                    }
                }
            }

            _logger.LogInformation("Retrieved {Count} of {TotalCount} deposit audit records", data.Count, totalCount);
            return (data, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching RBI Deposit Audit Dump paginated data");
            throw;
        }
    }

}
