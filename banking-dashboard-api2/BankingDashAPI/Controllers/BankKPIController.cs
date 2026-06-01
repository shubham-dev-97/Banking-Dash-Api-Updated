using Microsoft.AspNetCore.Mvc;
using BankingDashAPI.Services.Interfaces;
using BankingDashAPI.Models.Entities;
using System;
using System.Threading.Tasks;
using static BankingDashAPI.Models.Entities.BankKPIDashboard;

namespace BankingDashAPI.Controllers
{
    [ApiController]
    [Route("api/dashboard/[controller]")]
    public class BankKPIController : ControllerBase
    {
        private readonly IBankKPIService _bankKPIService;
        private readonly ILogger<BankKPIController> _logger;

        public BankKPIController(IBankKPIService bankKPIService, ILogger<BankKPIController> logger)
        {
            _bankKPIService = bankKPIService;
            _logger = logger;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetBankKPIDashboard(
            [FromQuery] string? finYear,
            [FromQuery] string? yearType,
            [FromQuery] string? regionName,
            [FromQuery] int? pbrcode,
            [FromQuery] int topRecords = 10)
        {
            try
            {
                var request = new BankKPIRequest
                {
                    FIN_YEAR = finYear,
                    YEAR_TYPE = yearType,
                    REGION_NAME = regionName,
                    PBRCODE = pbrcode,
                    TOP_RECORDS = topRecords
                };

                var result = await _bankKPIService.GetBankKPIDashboardAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetBankKPIDashboard");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}