using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Logic.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
            try
            {
                var results = _cityProcessor.GetCities();
                return Ok(results);
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while getting all cities.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }

        /// <summary>get a specific city by key</summary>
        /// <example>http://{domain}/api/v1.0/cities/{cityId}</example>
        /// <param name="cityId">the id of the city</param>
        /// <param name="includePointsOfInterest">flag which indicates whether or not points of interest should be included.  true/false</param>
        /// <returns>a CityDto with optional points of interest</returns>
        /// <response code="200">returns a city</response>
        /// <response code="404">city id not valid</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [HttpGet("{cityId}", Name = "GetCityById")]
        public ActionResult<CityDto> GetCityById([FromRoute] Guid cityId, [FromQuery] bool includePointsOfInterest = true)
        {
            try
            {
                if (!_cityProcessor.DoesCityExist(cityId))
                {
                    _logger.LogInformation($"**** LOGGER: City not found using cityId {cityId}.");
                    return NotFound($"City not found using cityId {cityId}.");
                }
                else
                {
                    var city = _cityProcessor.GetCityById(cityId, includePointsOfInterest);
                    return Ok(city);
                }
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while looking for a city: {cityId}.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }

        /// <summary>
        /// creates a new city
        /// </summary>
        /// <param name="newCity">content for new city in body</param>
        /// <returns>CityDto</returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        [HttpPost(Name = "CreateCity")]
        public ActionResult<CityDto> CreateCity([FromBody] CityCreateDto newCity)
        {
            try
            {
                if (newCity == null)
                {
                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                CityDto newCityDto = _cityProcessor.CreateCity(newCity);

                if (newCityDto == null)
                {
                    return StatusCode(500, "Something went wrong when creating a city.");
                }
                else
                {
                    // Returns 201 Created Status Code.
                    // Returns the ROUTE in the RESPONSE HEADER (http://localhost:49902/api/cities/{cityId}) where you can see it.
                    return CreatedAtRoute("GetCityById", new { cityId = newCityDto.CityId }, newCityDto);
                }
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while creating a city: {newCity}.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }

    }
}