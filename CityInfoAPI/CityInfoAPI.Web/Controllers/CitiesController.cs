using AutoMapper;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Logic.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CityInfoAPI.Web.Controllers
{
    /// <summary>cities controller</summary>
    /// <example>http://{domain}/api/v{version:apiVersion}/cities</example>
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
        private ICityInfoRepository _cityInfoRepository;


        /// <summary>constructor</summary>
        /// <param name="logger">logger factory middleware to be injected</param>
        /// <param name="cityProcessor">city processor middleware to be injected</param>
        /// <param name="cityInfoRepository">city repository</param>
        public CitiesController(ILogger<CitiesController> logger, CityProcessor cityProcessor, ICityInfoRepository cityInfoRepository)
        {
            _cityProcessor = cityProcessor;
            _logger = logger;
            _cityInfoRepository = cityInfoRepository;
        }

        /// <summary>get a collection of all cities. does not include point of interests.</summary>
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

        /// <summary>creates a new city</summary>
        /// <example>http://{domain}/api/v1.0/cities</example>
        /// <param name="newCity">content for new city in body</param>
        /// <returns>CityDto</returns>
        /// <response code="201">returns create at route location</response>
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

                // if the city has any points of interest in it's creation request, we need to pass
                // along that cityId to the points of interest.
                if (newCity.PointsOfInterest.Any())
                {
                    foreach (PointOfInterestCreateDto point in newCity.PointsOfInterest)
                    {
                        point.CityId = newCity.CityId;
                    }
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
                    // pass in the name of the route, any required args as a type, and the dto to be shown in the body.
                    return CreatedAtRoute("GetCityById", new { cityId = newCityDto.CityId }, newCityDto);
                }
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while creating a city: {newCity}.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }

        /// <summary>blocks a post to a city that already exists</summary>
        /// <example>http://localhost:5000/api/v1.0/cities/38276231-1918-452d-a3e9-6f50873a95d2</example>
        /// <param name="cityId">id of city</param>
        /// <returns>status codes or bad request</returns>
        /// <response code="409">warning - cannot post with id</response>
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesDefaultResponseType]
        [HttpPost("{cityId}", Name = "BlockPostToExistingCity")]
        public ActionResult BlockPostToExistingCity(Guid cityId)
        {
            // this is being a touch over-protective.  The idea is to not allow (and inform) the consumer
            // that they can post to this endpoint with an id.  Anything with an id should be done with a PUT
            // or a PATCH.
            try
            {
                if (!_cityProcessor.DoesCityExist(cityId))
                {
                    return BadRequest("You cannot post to cities like this.");
                }
                else
                {
                    return StatusCode(409, "You cannot post to an existing city!");
                }
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while posting to BlockPostToExistingCity: {cityId}.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }

        /// <summary>PATCH endpoint for updating city - can update name and description only</summary>
        /// <example>http://{domain}/api/v1.0/cities/{cityId}</example>
        /// <param name="cityId">required city key</param>
        /// <param name="patchDocument">required patch document which indicates which part(s) will be updated</param>
        /// <returns>returns CityUpdateDto with new value(s)</returns>
        /// <remarks>
        /// http://{domain}/api/{cityId}
        /// ```
        /// sample patch document. allows you to patch **name and/or description**
        /// [
        ///	    {
        ///		    "op": "replace",
        ///		    "path": "/description",
        ///		    "value": "new description of city"
        ///	    }
        ///]
        ///```
        /// </remarks>
        /// <response code="200">returns patched city</response>
        /// <response code="404">city id not valid</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [HttpPatch("{cityId}", Name = "PatchCity")]
        public ActionResult<CityUpdateDto> PatchCity(Guid cityId, [FromBody] JsonPatchDocument<CityUpdateDto> patchDocument)
        {
            try
            {
                // see if the correct properties and type was passed in
                if (patchDocument == null)
                {
                    return BadRequest();
                }

                // is this a valid city?
                if (!_cityProcessor.DoesCityExist(cityId))
                {
                    _logger.LogInformation($"**** LOGGER: City of cityId {cityId} was not found.");
                    return NotFound($"City of cityId {cityId} was not found.");
                }

                // we need to map the entity to a dto so we than can map the patch to the dto and back to the entity.
                // <casted destination type>(source).
                var cityEntity = _cityInfoRepository.GetCityById(cityId, false);
                var cityToPatch = Mapper.Map<CityUpdateDto>(cityEntity);

                // If we include the optional ModelState argument, it will send back any potential errors.
                // This is where we map new values to the properties.
                // ModelState was created here when the Model Binding was applied to the input model...the JSONPatchDocument.
                // Since the framework has no way of knowing what was required and valid in the document, it will usually have
                // no errors and be valid.
                patchDocument.ApplyTo(cityToPatch, ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // We can solve this with some custom logic again. are the name and description provided?
                if (string.IsNullOrWhiteSpace(cityToPatch.Name) || string.IsNullOrWhiteSpace(cityToPatch.Description))
                {
                    return BadRequest(ModelState);
                }

                // Try to validate the model again
                TryValidateModel(cityToPatch);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // all is good.  map the new values back to the entity
                Mapper.Map(cityToPatch, cityEntity);

                if (!_cityInfoRepository.SaveChanges())
                {
                    _logger.LogWarning($"**** LOGGER: An error occurred when patching the city: {cityId}.");
                    return StatusCode(500, $"An error occurred when patching the city: {cityId}.");
                }

                return Ok(cityToPatch);
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while trying to patch a city: {cityId}.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }
    }
}