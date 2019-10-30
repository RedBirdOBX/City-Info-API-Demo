using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Logic.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace CityInfoAPI.Web.Controllers
{
    /// <summary>cities controller</summary>
    /// <example>http://{domain}/api/cities</example>
    [Route("api/v{version:apiVersion}/cities")]
    [Produces("application/json", "application/xml")]
    [ApiController]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class CitiesController : ControllerBase
    {
        // fields
        private ILogger<CitiesController> _logger;
        private CityProcessor _cityProcessor;

        /// <summary>constructor</summary>
        /// <param name="logger">logger factory middleware to be injected</param>
        /// <param name="cityProcessor">city processor middleware to be injected</param>
        public CitiesController(ILogger<CitiesController> logger, CityProcessor cityProcessor)
        {
            _cityProcessor = cityProcessor;
            _logger = logger;
        }

        /// <summary>get a collection of all cities</summary>
        /// <example>http://{domain}/api/v1.0/cities</example>
        /// <returns>a collection of CityDto</returns>
        /// <response code="200">returns the list of cities</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        [HttpGet("", Name = "GetCities")]
        public ActionResult<List<CityDto>> GetCities()
        {
            var results = _cityProcessor.GetCities();
            return Ok(results);
        }

        /// <summary>get a specific city by key</summary>
        /// <example>http://{domain}/api/v1.0/cities/{cityKey}</example>
        /// <param name="cityKey">the key/guid of the city</param>
        /// <param name="includePointsOfInterest">flag which indicates whether or not points of interest should be included.  true/false</param>
        /// <returns>a CityDto with optional points of interest</returns>
        /// <response code="200">returns a city</response>
        /// <response code="404">city id not valid</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [HttpGet("{cityKey}", Name = "GetCityById")]
        public ActionResult<CityDto> GetCityByKey([FromRoute] string cityKey, [FromQuery] bool includePointsOfInterest = true)
        {
            if (!_cityProcessor.DoesCityExist(cityKey))
            {
                _logger.LogInformation($"**** LOGGER: City not found using key {cityKey}.");
                return NotFound($"City not found using cityKey {cityKey}.");
            }
            else
            {
                var city = _cityProcessor.GetCityByKey(cityKey, includePointsOfInterest);
                return Ok(city);
            }
        }
    }
}