using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Logic.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace CityInfoAPI.Web.Controllers
{
    /// <summary>controller for handling city collection requests</summary>
    /// <example>http://{domain}/api/v{version:apiVersion}/citycollections</example>
    [Route("api/v{version:apiVersion}/citycollections")]
    [Produces("application/json", "application/xml")]
    [ApiController]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class CityCollectionsController : ControllerBase
    {
        // fields
        private ILogger<CitiesController> _logger;
        private CityCollectionsProcessor _cityCollectionsProcessor;
        private CityProcessor _cityProcessor;

        /// <summary>constructor</summary>
        /// <param name="logger"></param>
        /// <param name="cityCollectionsProcessor"></param>
        /// <param name="cityProcessor"></param>
        public CityCollectionsController(ILogger<CitiesController> logger, CityCollectionsProcessor cityCollectionsProcessor, CityProcessor cityProcessor)
        {
            _logger = logger;
            _cityCollectionsProcessor = cityCollectionsProcessor;
            _cityProcessor = cityProcessor;
        }

        /// <summary>returns a collection of cities via ids in query-string</summary>
        /// <example>http://localhost:5000/api/v1.0/citycollections?cityIds={guid1,guid2,guid3}</example>
        /// <param name="cityIds">comma delimited list of city ids (guids)</param>
        /// <returns>collection of city dtos</returns>
        /// <response code="200">returns collection of cities</response>
        /// <response code="404">returns not found</response>
        [HttpGet("", Name ="GetCitiesById")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<CityDto>> GetCitiesById([FromQuery] string cityIds)
        {
            if (cityIds == null)
            {
                ModelState.AddModelError("Description", "CityIds parameter is missing.");
                return BadRequest(ModelState);
            }

            List<CityDto> results = _cityCollectionsProcessor.GetCities(cityIds);

            try
            {
                if (results.Count < 1)
                {
                    _logger.LogInformation($"**** LOGGER: Cities not found with ids {cityIds}.");
                    return NotFound($"Cities not found with ids {cityIds}.");
                }
                return Ok(results);

            }
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while looking for a cities: {cityIds}.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }

        /// <summary>creates multiple cities with single post</summary>
        /// <example>http://localhost:5000/api/v1.0/citycollections</example>
        /// <param name="submittedCities"></param>
        /// <returns>response and endpoint where new cities can be found</returns>
        /// <response code="201">returns location of new cities</response>
        [HttpPost("", Name = "CreateCities")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<List<CityDto>> CreateCityCollections([FromBody] List<CityCreateDto> submittedCities)
        {
            // if collection doesn't map to post
            if (submittedCities == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                List<CityDto> newCities = _cityCollectionsProcessor.CreateCities(submittedCities);

                if (newCities == null)
                {
                    return StatusCode(500, "Something went wrong when creating a collection of cities.");
                }
                else
                {
                    // build a string for the query-string
                    string qsCityIds = string.Join(",", newCities.Select(c => c.CityId));

                    // Returns 201 Created Status Code.
                    // Returns the ROUTE in the RESPONSE HEADER (http://localhost:5000/api/v1.0/citycollections?cityIds={a,b,c}) where you can see it.
                    // pass in the name of the route, any required args as a type, and the dto to be shown in the body.
                    return CreatedAtRoute("GetCitiesById", new { cityIds = qsCityIds }, newCities);
                }
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while creating cities. {submittedCities.Select(c => c.Name).ToList()}.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }

        }
    }
}