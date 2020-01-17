using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Logic.Processors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CityInfoAPI.Web.Controllers
{
    /// <summary>returns various reports. controller requires a version to be specified</summary>
    [Route("api/v{version:apiVersion}/cities/reporting")]
    [Produces("application/json", "application/xml")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ApiVersion("2.0")]
    [Authorize]
    public class ReportingController : ControllerBase
    {

        private ReportingProcessor _reportingProcessor;
        private ILogger<ReportingController> _logger;

        /// <summary>constructor</summary>
        /// <param name="reportingProcessor">reporting processor</param>
        /// <param name="logger">logger factory to be injected</param>
        public ReportingController(ReportingProcessor reportingProcessor, ILogger<ReportingController> logger)
        {
            _reportingProcessor = reportingProcessor;
            _logger = logger;
        }


        /// <summary>V2 resource. returns a list of cities and its count of points of interest</summary>
        /// <example>http://{domain}/api/v2.0/cities/reporting/summary</example>
        /// <returns>collection of city summary dto</returns>
        /// <response code="200">returns point of interest for city</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        [HttpGet("summary", Name = "GetCitySummary")]
        public async Task<ActionResult<List<CitySummaryDto>>> CitySummary()
        {
            try
            {
                var results = await _reportingProcessor.GetCitiesSummary();
                return results;
            }
            catch (System.Exception exception)
            {
                _logger.LogWarning($"**** LOGGER: An error occurred in the CitySummary action. {exception}");
                return StatusCode(500, "An error occurred when patching the point of interest.");
            }
        }
    }
}