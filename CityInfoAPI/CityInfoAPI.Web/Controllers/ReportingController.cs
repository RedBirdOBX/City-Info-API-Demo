using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Logic.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CityInfoAPI.Web.Controllers
{
    /// <summary>returns various reports. controller requires a version to be specified</summary>
    [Route("api/v{version:apiVersion}/cities/reporting")]
    [Produces("application/json", "application/xml")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ApiVersion("2.0")]
    public class ReportingController : ControllerBase
    {

        private ReportingProcessor _reportingProcessor;

        /// <summary>constructor</summary>
        /// <param name="reportingProcessor">reporting processor</param>
        public ReportingController(ReportingProcessor reportingProcessor)
        {
            _reportingProcessor = reportingProcessor;
        }


        /// <summary>V2 resource. returns a list of cities and its count of points of interest</summary>
        /// <example>http://{domain}/api/cities/reporting/summary</example>
        /// <returns>collection of city summary dto</returns>
        /// <response code="200">returns point of interest for city</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        [HttpGet("summary", Name = "GetCitySummary")]
        public ActionResult<List<CitySummaryDto>> CitySummary()
        {
            var results = _reportingProcessor.GetCitiesSummary();
            return results;
        }
    }
}