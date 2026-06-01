using BankingDashAPI.Models.Entities;
using BankingDashAPI.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static BankingDashAPI.Models.Entities.BankKPIDashboard;

namespace BankingDashAPI.Services
{
    public class BankKPIService : IBankKPIService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BankKPIService> _logger;
        private readonly string _connectionString;

        public BankKPIService(IConfiguration configuration, ILogger<BankKPIService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<BankKPIDashboardResponse> GetBankKPIDashboardAsync(BankKPIRequest request)
        {
            var response = new BankKPIDashboardResponse();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("USP_GET_BANK_KPI_DASHBOARD", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FIN_YEAR", request.FIN_YEAR ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@YEAR_TYPE", request.YEAR_TYPE ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@REGION_NAME", request.REGION_NAME ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@PBRCODE", request.PBRCODE ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TOP_RECORDS", request.TOP_RECORDS);
                        command.CommandTimeout = 120;

                        await connection.OpenAsync();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            // Result Set 1: KPI Summary
                            if (await reader.ReadAsync())
                            {
                                response.Summary = new BankKPISummary
                                {
                                    TOTAL_BRANCHES = ConvertToInt32(reader.GetValue(0)),
                                    TOTAL_CUSTOMERS = ConvertToDecimal(reader.GetValue(1)),
                                    ACTIVE_CUSTOMERS = ConvertToDecimal(reader.GetValue(2)),
                                    NEW_CUSTOMERS = ConvertToDecimal(reader.GetValue(3)),
                                    TOTAL_DEPOSIT_CR = ConvertToDecimal(reader.GetValue(4)),
                                    TOTAL_LOAN_CR = ConvertToDecimal(reader.GetValue(5)),
                                    TOTAL_RECOVERY_CR = ConvertToDecimal(reader.GetValue(6)),
                                    AVG_GROSS_NPA = ConvertToDecimal(reader.GetValue(7)),
                                    AVG_NET_NPA = ConvertToDecimal(reader.GetValue(8)),
                                    AVG_CASA_RATIO = ConvertToDecimal(reader.GetValue(9)),
                                    DIGITAL_PERCENT = ConvertToDecimal(reader.GetValue(10)),
                                    AVG_PERFORMANCE = ConvertToDecimal(reader.GetValue(11)),
                                    TOTAL_UPI_TRANSACTION_CR = ConvertToDecimal(reader.GetValue(12))
                                };
                            }

                            // Result Set 2: Yearly Summary
                            await reader.NextResultAsync();
                            var yearlySummary = new List<CEOYearlySummary>();
                            while (await reader.ReadAsync())
                            {
                                yearlySummary.Add(new CEOYearlySummary
                                {
                                    FIN_YEAR = ConvertToString(reader.GetValue(0)),
                                    TOTAL_BRANCHES = ConvertToInt32(reader.GetValue(1)),
                                    TOTAL_DEPOSIT_CR = ConvertToDecimal(reader.GetValue(2)),
                                    TOTAL_LOAN_CR = ConvertToDecimal(reader.GetValue(3)),
                                    TOTAL_RECOVERY_CR = ConvertToDecimal(reader.GetValue(4)),
                                    AVG_GROSS_NPA = ConvertToDecimal(reader.GetValue(5)),
                                    AVG_NET_NPA = ConvertToDecimal(reader.GetValue(6)),
                                    DIGITAL_PERCENT = ConvertToDecimal(reader.GetValue(7)),
                                    AVG_PERFORMANCE = ConvertToDecimal(reader.GetValue(8))
                                });
                            }
                            response.YearlySummary = yearlySummary;

                            // Result Set 3: Region Summary
                            await reader.NextResultAsync();
                            var regionSummary = new List<RegionKPISummary>();
                            while (await reader.ReadAsync())
                            {
                                regionSummary.Add(new RegionKPISummary
                                {
                                    REGION_NAME = ConvertToString(reader.GetValue(0)),
                                    TOTAL_BRANCHES = ConvertToInt32(reader.GetValue(1)),
                                    TOTAL_DEPOSIT_CR = ConvertToDecimal(reader.GetValue(2)),
                                    TOTAL_LOAN_CR = ConvertToDecimal(reader.GetValue(3)),
                                    TOTAL_RECOVERY_CR = ConvertToDecimal(reader.GetValue(4)),
                                    AVG_NPA = ConvertToDecimal(reader.GetValue(5)),
                                    DIGITAL_PERCENT = ConvertToDecimal(reader.GetValue(6)),
                                    AVG_PERFORMANCE = ConvertToDecimal(reader.GetValue(7))
                                });
                            }
                            response.RegionSummary = regionSummary;

                            // Result Set 4: Top Branches
                            await reader.NextResultAsync();
                            var topBranches = new List<BranchPerformanceItem>();
                            while (await reader.ReadAsync())
                            {
                                topBranches.Add(new BranchPerformanceItem
                                {
                                    PBRCODE = ConvertToString(reader.GetValue(0)),
                                    BRANCH_NAME = ConvertToString(reader.GetValue(1)),
                                    REGION_NAME = ConvertToString(reader.GetValue(2)),
                                    TOTAL_DEPOSIT_ACHIEVED_CR = ConvertToDecimal(reader.GetValue(3)),
                                    TOTAL_LOAN_ACHIEVED_CR = ConvertToDecimal(reader.GetValue(4)),
                                    RECOVERY_ACHIEVED_CR = ConvertToDecimal(reader.GetValue(5)),
                                    GROSS_NPA_PERCENT = ConvertToDecimal(reader.GetValue(6)),
                                    DIGITAL_TRANSACTION_PERCENT = ConvertToDecimal(reader.GetValue(7)),
                                    OVERALL_ACHIEVEMENT_PERCENT = ConvertToDecimal(reader.GetValue(8)),
                                    PERFORMANCE_STATUS = ConvertToString(reader.GetValue(9))
                                });
                            }
                            response.TopBranches = topBranches;

                            // Result Set 5: Bottom Branches
                            await reader.NextResultAsync();
                            var bottomBranches = new List<BranchPerformanceItem>();
                            while (await reader.ReadAsync())
                            {
                                bottomBranches.Add(new BranchPerformanceItem
                                {
                                    PBRCODE = ConvertToString(reader.GetValue(0)),
                                    BRANCH_NAME = ConvertToString(reader.GetValue(1)),
                                    REGION_NAME = ConvertToString(reader.GetValue(2)),
                                    TOTAL_DEPOSIT_ACHIEVED_CR = ConvertToDecimal(reader.GetValue(3)),
                                    TOTAL_LOAN_ACHIEVED_CR = ConvertToDecimal(reader.GetValue(4)),
                                    RECOVERY_ACHIEVED_CR = ConvertToDecimal(reader.GetValue(5)),
                                    GROSS_NPA_PERCENT = ConvertToDecimal(reader.GetValue(6)),
                                    DIGITAL_TRANSACTION_PERCENT = ConvertToDecimal(reader.GetValue(7)),
                                    OVERALL_ACHIEVEMENT_PERCENT = ConvertToDecimal(reader.GetValue(8)),
                                    PERFORMANCE_STATUS = ConvertToString(reader.GetValue(9))
                                });
                            }
                            response.BottomBranches = bottomBranches;

                            // Result Set 6: Branch Detail Grid
                            await reader.NextResultAsync();
                            var branchGrid = new List<BranchDetailGrid>();
                            while (await reader.ReadAsync())
                            {
                                branchGrid.Add(new BranchDetailGrid
                                {
                                    PBRCODE = ConvertToString(reader.GetValue(0)),
                                    BRANCH_NAME = ConvertToString(reader.GetValue(1)),
                                    REGION_NAME = ConvertToString(reader.GetValue(2)),
                                    TOTAL_CUSTOMERS = ConvertToDecimal(reader.GetValue(3)),
                                    ACTIVE_CUSTOMERS = ConvertToDecimal(reader.GetValue(4)),
                                    NEW_CUSTOMERS = ConvertToDecimal(reader.GetValue(5)),
                                    STAFF_COUNT = ConvertToInt32(reader.GetValue(6)),
                                    TOTAL_DEPOSIT_TARGET_CR = ConvertToDecimal(reader.GetValue(7)),
                                    TOTAL_DEPOSIT_ACHIEVED_CR = ConvertToDecimal(reader.GetValue(8)),
                                    CASA_TARGET_CR = ConvertToDecimal(reader.GetValue(9)),
                                    CASA_ACHIEVED_CR = ConvertToDecimal(reader.GetValue(10)),
                                    TERM_DEPOSIT_TARGET_CR = ConvertToDecimal(reader.GetValue(11)),
                                    TERM_DEPOSIT_ACHIEVED_CR = ConvertToDecimal(reader.GetValue(12)),
                                    CASA_RATIO_PERCENT = ConvertToDecimal(reader.GetValue(13)),
                                    TOTAL_LOAN_TARGET_CR = ConvertToDecimal(reader.GetValue(14)),
                                    TOTAL_LOAN_ACHIEVED_CR = ConvertToDecimal(reader.GetValue(15)),
                                    MSME_LOAN_CR = ConvertToDecimal(reader.GetValue(16)),
                                    GOLD_LOAN_CR = ConvertToDecimal(reader.GetValue(17)),
                                    RECOVERY_TARGET_CR = ConvertToDecimal(reader.GetValue(18)),
                                    RECOVERY_ACHIEVED_CR = ConvertToDecimal(reader.GetValue(19)),
                                    GROSS_NPA_PERCENT = ConvertToDecimal(reader.GetValue(20)),
                                    NET_NPA_PERCENT = ConvertToDecimal(reader.GetValue(21)),
                                    MOBILE_BANKING_CUSTOMERS = ConvertToDecimal(reader.GetValue(22)),
                                    INTERNET_BANKING_CUSTOMERS = ConvertToDecimal(reader.GetValue(23)),
                                    UPI_TRANSACTION_CR = ConvertToDecimal(reader.GetValue(24)),
                                    DIGITAL_TRANSACTION_PERCENT = ConvertToDecimal(reader.GetValue(25)),
                                    OVERALL_ACHIEVEMENT_PERCENT = ConvertToDecimal(reader.GetValue(26)),
                                    PERFORMANCE_STATUS = ConvertToString(reader.GetValue(27)),
                                    BRANCH_RANK = ConvertToInt32(reader.GetValue(28))
                                });
                            }
                            response.BranchGrid = branchGrid;

                            // Result Set 7: Map Dataset
                            await reader.NextResultAsync();
                            var mapData = new List<BranchMapData>();
                            while (await reader.ReadAsync())
                            {
                                mapData.Add(new BranchMapData
                                {
                                    PBRCODE = ConvertToString(reader.GetValue(0)),
                                    BRANCH_NAME = ConvertToString(reader.GetValue(1)),
                                    REGION_NAME = ConvertToString(reader.GetValue(2)),
                                    LATITUDE = reader.IsDBNull(3) ? null : ConvertToDecimal(reader.GetValue(3)),
                                    LONGITUDE = reader.IsDBNull(4) ? null : ConvertToDecimal(reader.GetValue(4)),
                                    TOTAL_DEPOSIT_ACHIEVED_CR = ConvertToDecimal(reader.GetValue(5)),
                                    TOTAL_LOAN_ACHIEVED_CR = ConvertToDecimal(reader.GetValue(6)),
                                    RECOVERY_ACHIEVED_CR = ConvertToDecimal(reader.GetValue(7)),
                                    GROSS_NPA_PERCENT = ConvertToDecimal(reader.GetValue(8)),
                                    DIGITAL_TRANSACTION_PERCENT = ConvertToDecimal(reader.GetValue(9)),
                                    OVERALL_ACHIEVEMENT_PERCENT = ConvertToDecimal(reader.GetValue(10)),
                                    PERFORMANCE_STATUS = ConvertToString(reader.GetValue(11)),
                                    GOOGLE_MAP_LOCATION = ConvertToString(reader.GetValue(12))
                                });
                            }
                            response.MapData = mapData;

                            // Result Set 8: Trend Analysis
                            await reader.NextResultAsync();
                            var trendAnalysis = new List<KPIYearlyTrend>();
                            while (await reader.ReadAsync())
                            {
                                trendAnalysis.Add(new KPIYearlyTrend
                                {
                                    FIN_YEAR = ConvertToString(reader.GetValue(0)),
                                    TOTAL_DEPOSIT_CR = ConvertToDecimal(reader.GetValue(1)),
                                    TOTAL_LOAN_CR = ConvertToDecimal(reader.GetValue(2)),
                                    TOTAL_RECOVERY_CR = ConvertToDecimal(reader.GetValue(3)),
                                    AVG_NPA = ConvertToDecimal(reader.GetValue(4)),
                                    DIGITAL_PERCENT = ConvertToDecimal(reader.GetValue(5)),
                                    AVG_PERFORMANCE = ConvertToDecimal(reader.GetValue(6))
                                });
                            }
                            response.TrendAnalysis = trendAnalysis;

                            // Result Set 9: Actual vs Projection
                            await reader.NextResultAsync();
                            var actualVsProjection = new List<ActualVsProjection>();
                            while (await reader.ReadAsync())
                            {
                                actualVsProjection.Add(new ActualVsProjection
                                {
                                    YEAR_TYPE = ConvertToString(reader.GetValue(0)),
                                    TOTAL_DEPOSIT_CR = ConvertToDecimal(reader.GetValue(1)),
                                    TOTAL_LOAN_CR = ConvertToDecimal(reader.GetValue(2)),
                                    TOTAL_RECOVERY_CR = ConvertToDecimal(reader.GetValue(3)),
                                    AVG_NPA = ConvertToDecimal(reader.GetValue(4)),
                                    DIGITAL_PERCENT = ConvertToDecimal(reader.GetValue(5)),
                                    AVG_PERFORMANCE = ConvertToDecimal(reader.GetValue(6))
                                });
                            }
                            response.ActualVsProjection = actualVsProjection;
                        }
                    }
                }

                _logger.LogInformation("Bank KPI Dashboard data retrieved successfully");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Bank KPI Dashboard data");
                throw;
            }
        }

        // Helper methods
        private int ConvertToInt32(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;
            return Convert.ToInt32(value);
        }

        private decimal ConvertToDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;
            return Convert.ToDecimal(value);
        }

        private string ConvertToString(object value)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;
            return value.ToString() ?? string.Empty;
        }
    }
}