using BankingDashAPI.Models.DTOs;
using BankingDashAPI.Models.Filters;
using BankingDashAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BankingDashAPI.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService service, ILogger<DashboardController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("home-kpi")]
    public async Task<IActionResult> GetHomeKpi([FromQuery] HomeFilter filter)
        => Ok(await _service.GetHomeKpi(filter));

    [HttpGet("deposit-analysis")]
    public async Task<IActionResult> GetDepositAnalysis([FromQuery] DepositFilter filter)
        => Ok(await _service.GetDepositAnalysis(filter));

    [HttpGet("loan-analysis")]
    public async Task<IActionResult> GetLoanAnalysis([FromQuery] LoanFilter filter)
        => Ok(await _service.GetLoanAnalysis(filter));

    [HttpGet("monthly-trend")]
    public async Task<IActionResult> GetMonthlyTrend([FromQuery] MonthlyTrendFilter filter)
        => Ok(await _service.GetMonthlyTrend(filter));

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] SummaryFilter filter)
        => Ok(await _service.GetBankingSummary(filter));




    [HttpGet("customer-count-by-category")]
    public async Task<IActionResult> GetCustomerCountByCategory()
    {
        try
        {
            
            DateTime targetDate = new DateTime(2025, 3, 31);

            var filter = new CustomerCountFilter
            {
                AsOnDate = targetDate
            };

            var data = await _service.GetCustomerCountByCategory(filter);

           
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = ex.Message,
                stackTrace = ex.StackTrace
            });
        }
    }



    // For the Summary

    [HttpGet("home-customer-summary")]
    [ProducesResponseType(typeof(HomeCustomerSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHomeCustomerSummary([FromQuery] DateTime? asOnDate)
    {
        try
        {
            _logger.LogInformation("Getting home customer summary for date: {Date}", asOnDate);

            DateTime targetDate = asOnDate ?? new DateTime(2025, 3, 31);
            var summary = await _service.GetHomeCustomerSummary(targetDate);

            _logger.LogInformation("Successfully got summary: {@Summary}", summary);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching home customer summary. Details: {Message}", ex.Message);
            // Return the actual error message for debugging
            return StatusCode(500, new
            {
                error = ex.Message,
                innerError = ex.InnerException?.Message,
                stackTrace = ex.StackTrace
            });
        }
    }

    // Fro the Available Dates
    [HttpGet("available-dates")]
    public IActionResult GetAvailableDates()
    {
        try
        {
            var dates = _service.GetAvailableDates();

            // Convert DateTime to string format YYYY-MM-DD for frontend
            var dateStrings = dates.Select(d => d.ToString("yyyy-MM-dd")).ToList();

            return Ok(dateStrings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAvailableDates endpoint");
            return StatusCode(500, new { error = ex.Message });
        }
    }


    // Get the Deposit Opening Summary

    [HttpGet("deposit-opening-summary")]
    public IActionResult GetDepositOpeningSummary([FromQuery] DateTime asOnDate)
    {
        try
        {
            var summary = _service.GetDepositOpeningSummary(asOnDate);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetDepositOpeningSummary");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}