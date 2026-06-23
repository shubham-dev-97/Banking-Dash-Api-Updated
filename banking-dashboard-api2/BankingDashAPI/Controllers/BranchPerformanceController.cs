using Microsoft.AspNetCore.Mvc;
using BankingDashAPI.Services.Interfaces;
using static BankingDashAPI.Models.Entities.BranchPerformanceDashboard;

namespace BankingDashAPI.Controllers
{
    [ApiController]
    [Route("api/dashboard/[controller]")]
    public class BranchPerformanceController : ControllerBase
    {
        private readonly IBranchPerformanceService _branchPerformanceService;
        private readonly ILogger<BranchPerformanceController> _logger;

        public BranchPerformanceController(
            IBranchPerformanceService branchPerformanceService,
            ILogger<BranchPerformanceController> logger)
        {
            _branchPerformanceService = branchPerformanceService;
            _logger = logger;
        }


        [HttpGet("dashboard")]
        public async Task<IActionResult> GetBranchPerformanceDashboard(
            [FromQuery] DateTime? targetDate,
            [FromQuery] string? regionName,
            [FromQuery] string? performanceStatus)
        {
            try
            {
                _logger.LogInformation("GetBranchPerformanceDashboard called with targetDate: {TargetDate}, regionName: {RegionName}, performanceStatus: {PerformanceStatus}",
                    targetDate, regionName, performanceStatus);

                var request = new BranchPerformanceRequest
                {
                    TARGET_DATE = targetDate,
                    REGION_NAME = regionName,
                    PERFORMANCE_STATUS = performanceStatus
                };

                var result = await _branchPerformanceService.GetBranchPerformanceDashboardAsync(request);

                _logger.LogInformation("GetBranchPerformanceDashboard completed successfully");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetBranchPerformanceDashboard");
                return StatusCode(500, new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerError = ex.InnerException?.Message
                });
            }
        }
    }
}