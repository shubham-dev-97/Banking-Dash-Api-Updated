using BankingDashAPI.Models.Entities;
using BankingDashAPI.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static BankingDashAPI.Models.Entities.BranchPerformanceDashboard;

namespace BankingDashAPI.Services
{
    public class BranchPerformanceService : IBranchPerformanceService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BranchPerformanceService> _logger;
        private readonly string _connectionString;

        public BranchPerformanceService(IConfiguration configuration, ILogger<BranchPerformanceService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<BranchPerformanceDashboardResponse> GetBranchPerformanceDashboardAsync(BranchPerformanceRequest request)
        {
            var response = new BranchPerformanceDashboardResponse();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("USP_GET_BRANCH_PERFORMANCE_DASHBOARD", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TARGET_DATE", request.TARGET_DATE ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@REGION_NAME", request.REGION_NAME ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@PERFORMANCE_STATUS", request.PERFORMANCE_STATUS ?? (object)DBNull.Value);
                        command.CommandTimeout = 60;

                        await connection.OpenAsync();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            // Result Set 1: Summary Cards
                            if (await reader.ReadAsync())
                            {
                                response.Summary = new BranchPerformanceSummary
                                {
                                    TOTAL_BRANCHES = ConvertToInt32(reader.GetValue(0)),
                                    TOTAL_DAILY_RECOVERY_TARGET = ConvertToDecimal(reader.GetValue(1)),
                                    TOTAL_DAILY_RECOVERY_ACHIEVED = ConvertToDecimal(reader.GetValue(2)),
                                    DAILY_RECOVERY_PERCENT = ConvertToDecimal(reader.GetValue(3)),
                                    TOTAL_CASA_TARGET = ConvertToDecimal(reader.GetValue(4)),
                                    TOTAL_CASA_ACHIEVED = ConvertToDecimal(reader.GetValue(5)),
                                    CASA_PERCENT = ConvertToDecimal(reader.GetValue(6)),
                                    TOTAL_TERM_DEPOSIT_TARGET = ConvertToDecimal(reader.GetValue(7)),
                                    TOTAL_TERM_DEPOSIT_ACHIEVED = ConvertToDecimal(reader.GetValue(8)),
                                    TERM_DEPOSIT_PERCENT = ConvertToDecimal(reader.GetValue(9)),
                                    TOTAL_NEW_CUSTOMERS = ConvertToInt32(reader.GetValue(10)),
                                    TOTAL_MOBILE_BANKING_CUSTOMERS = ConvertToInt32(reader.GetValue(11)),
                                    AVG_NPA_PERCENT = ConvertToDecimal(reader.GetValue(12)),
                                    AVG_OVERALL_ACHIEVEMENT = ConvertToDecimal(reader.GetValue(13)),
                                    LAST_UPDATED = ConvertToDateTime(reader.GetValue(14))
                                };
                            }
                            else
                            {
                                response.Summary = new BranchPerformanceSummary();
                            }

                            // Result Set 2: Branch Performance Grid
                            await reader.NextResultAsync();
                            var branchGrid = new List<BranchPerformanceGrid>();
                            while (await reader.ReadAsync())
                            {
                                var gridItem = new BranchPerformanceGrid();

                                gridItem.BRANCH_RANK = ConvertToInt32(reader.GetValue(0));
                                gridItem.PBRCODE = ConvertToString(reader.GetValue(1));
                                gridItem.BRANCH_NAME = ConvertToString(reader.GetValue(2));
                                gridItem.BRANCH_MANAGER = ConvertToString(reader.GetValue(3));
                                gridItem.DAILY_RECOVERY_TARGET = ConvertToDecimal(reader.GetValue(4));
                                gridItem.DAILY_RECOVERY_ACHIEVED = ConvertToDecimal(reader.GetValue(5));
                                gridItem.DAILY_RECOVERY_PERCENT = ConvertToDecimal(reader.GetValue(6));
                                gridItem.CASA_TARGET_CR = ConvertToDecimal(reader.GetValue(7));
                                gridItem.CASA_ACHIEVED_CR = ConvertToDecimal(reader.GetValue(8));
                                gridItem.CASA_PERCENT = ConvertToDecimal(reader.GetValue(9));
                                gridItem.TERM_DEPOSIT_TARGET_CR = ConvertToDecimal(reader.GetValue(10));
                                gridItem.TERM_DEPOSIT_ACHIEVED_CR = ConvertToDecimal(reader.GetValue(11));
                                gridItem.TERM_DEPOSIT_PERCENT = ConvertToDecimal(reader.GetValue(12));
                                gridItem.NEW_CUSTOMERS = ConvertToInt32(reader.GetValue(13));
                                gridItem.MOBILE_BANKING_CUSTOMERS = ConvertToInt32(reader.GetValue(14));
                                gridItem.NPA_PERCENT = ConvertToDecimal(reader.GetValue(15));
                                gridItem.OVERALL_ACHIEVEMENT_PERCENT = ConvertToDecimal(reader.GetValue(16));
                                gridItem.PERFORMANCE_STATUS = ConvertToString(reader.GetValue(17));
                                gridItem.STATUS_COLOR = ConvertToString(reader.GetValue(18));

                                branchGrid.Add(gridItem);
                            }
                            response.BranchGrid = branchGrid;

                            // Result Set 3: Region Summary
                            await reader.NextResultAsync();
                            var regionSummary = new List<RegionSummary>();
                            while (await reader.ReadAsync())
                            {
                                var region = new RegionSummary();
                                region.REGION_NAME = ConvertToString(reader.GetValue(0));
                                region.TOTAL_BRANCHES = ConvertToInt32(reader.GetValue(1));
                                region.TOTAL_DEPOSIT = ConvertToDecimal(reader.GetValue(2));
                                region.TOTAL_LOAN = ConvertToDecimal(reader.GetValue(3));
                                region.TOTAL_RECOVERY = ConvertToDecimal(reader.GetValue(4));
                                region.AVG_PERFORMANCE_PERCENT = ConvertToDecimal(reader.GetValue(5));
                                regionSummary.Add(region);
                            }
                            response.RegionSummary = regionSummary;

                            // Result Set 4: Top 10 Branches
                            await reader.NextResultAsync();
                            var topBranches = new List<TopBranch>();
                            while (await reader.ReadAsync())
                            {
                                var topBranch = new TopBranch();
                                topBranch.BRANCH_RANK = ConvertToInt32(reader.GetValue(0));
                                topBranch.PBRCODE = ConvertToString(reader.GetValue(1));
                                topBranch.BRANCH_NAME = ConvertToString(reader.GetValue(2));
                                topBranch.BRANCH_MANAGER = ConvertToString(reader.GetValue(3));
                                topBranch.OVERALL_ACHIEVEMENT_PERCENT = ConvertToDecimal(reader.GetValue(4));
                                topBranch.PERFORMANCE_STATUS = ConvertToString(reader.GetValue(5));
                                topBranch.NPA_PERCENT = ConvertToDecimal(reader.GetValue(6));
                                topBranches.Add(topBranch);
                            }
                            response.TopBranches = topBranches;
                        }
                    }
                }

                _logger.LogInformation("Branch performance dashboard data retrieved successfully");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching branch performance dashboard data");
                throw;
            }
        }

        // Helper methods for safe type conversion
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

        private DateTime ConvertToDateTime(object value)
        {
            if (value == null || value == DBNull.Value)
                return DateTime.Now;
            return Convert.ToDateTime(value);
        }

        private string ConvertToString(object value)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;
            return value.ToString() ?? string.Empty;
        }
    }
}