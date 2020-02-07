using AutoMapper;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Logic.Processors;
using CityInfoAPI.Web.Controllers.RequestHelpers;
using CityInfoAPI.Web.Controllers.ResponseHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        private PointsOfInterestProcessor _pointsOfInterestProcessor;
        private ICityInfoRepository _cityInfoRepository;
        private LinkGenerator _linkGenerator;
        private IHttpContextAccessor _httpContextAccessor;


        /// <summary>constructor</summary>
        /// <param name="logger">logger factory middleware to be injected</param>
        /// <param name="cityProcessor">city processor middleware to be injected</param>
        /// <param name="poiProcessor">points of interest processor middleware to be injected</param>
        /// <param name="cityRepository">city repository</param>
        /// <param name="httpContextAccessor">httpContextAccessor needed for LinkGenerator</param>
        /// <param name="linkGenerator">link generator</param>
        public CitiesController(ILogger<CitiesController> logger, CityProcessor cityProcessor, PointsOfInterestProcessor poiProcessor, ICityInfoRepository cityRepository, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            _cityProcessor = cityProcessor;
            _logger = logger;
            _cityInfoRepository = cityRepository;
            _pointsOfInterestProcessor = poiProcessor;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
        }

        /// <summary>gets collection of cities - results are paged</summary>
        /// <example>http://{domain}/api/v1.0/cities?pageNumber=1{n}_and_pageSize={n}</example>
        /// <param name="requestParameters"></param>
        /// <returns>collection of cities w/ no points of interest</returns>
        /// <response code="200">returns collection of cities</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        [HttpGet("", Name = "GetPagedCities")]
        public async Task<ActionResult<List<CityDto>>> GetPagedCities([FromQuery] RequestParameters requestParameters)
        {
            try
            {
                // get only the cities we need
                var pagedCities = await _cityProcessor.GetPagedCities(requestParameters.PageNumber, requestParameters.PageSize, requestParameters.Name);

                // how many total pages do we have?
                int allPossibleCities = 0;

                // did they use a name filter? count all possible results
                if (!string.IsNullOrEmpty(requestParameters.Name))
                {
                    var allCities = await _cityProcessor.GetAllCities();
                    var filteredCities = allCities.Where(c => c.Name.ToLower().Contains(requestParameters.Name.ToLower()));
                    allPossibleCities = filteredCities.Count();
                }
                else
                {
                    // count all
                    allPossibleCities = _cityProcessor.GetAllCities().Result.Count();
                }

                int totalPages = (int)Math.Ceiling(allPossibleCities / (double)requestParameters.PageSize);

                if (requestParameters.PageNumber > totalPages)
                {
                    return BadRequest("You have requested more pages that are available.");
                }

                // build the meta-data to return in header
                var metaData = MetaDataHelper.BuildCitiesMetaData(requestParameters, _cityProcessor, _httpContextAccessor, _linkGenerator);

                // add as custom header
                Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(metaData));

                return Ok(pagedCities);
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
        public async Task<ActionResult<CityDto>> GetCityById([FromRoute] Guid cityId, [FromQuery] bool includePointsOfInterest = true)
        {
            try
            {
                bool doesCityExist = await _cityProcessor.DoesCityExist(cityId);
                if (!doesCityExist)
                {
                    _logger.LogInformation($"**** LOGGER: City not found using cityId {cityId}.");
                    return NotFound($"City not found using cityId {cityId}.");
                }
                else
                {
                    _logger.LogInformation($"**** LOGGER: Consumer looking for cityId {cityId}.");
                    var city = await _cityProcessor.GetCityById(cityId, includePointsOfInterest);
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
        /// <param name="newCityRequest">content for new city in body</param>
        /// <returns>CityDto</returns>
        /// <response code="201">returns create at route location</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        [HttpPost(Name = "CreateCity")]
        public async Task<ActionResult<CityDto>> CreateCity([FromBody] CityCreateDto newCityRequest)
        {
            try
            {
                // Does a city with this name already exist?
                List<CityWithoutPointsOfInterestDto> allCities = await _cityProcessor.GetAllCities();
                if (allCities.Where(c => c.Name.ToLower() == newCityRequest.Name.Trim().ToLower()).Count() > 0)
                {
                    ModelState.AddModelError("Description", "A city with this name already exists.");
                    return BadRequest(ModelState);
                }

                // make sure name and description are unique
                if (newCityRequest.Name.ToLower().Equals(newCityRequest.Description.ToLower()))
                {
                    ModelState.AddModelError("Description", "Name and Description cannot be the same.");
                    return BadRequest(ModelState);
                }

                // **only** create the city.  ignore points of interest for now
                CityCreateDto createCityDto = new CityCreateDto()
                {
                    CityId = newCityRequest.CityId,
                    Name = newCityRequest.Name,
                    Description = newCityRequest.Description
                };

                // create the new city
                CityDto newCityDto = await _cityProcessor.CreateCity(createCityDto);

                if (newCityDto == null)
                {
                    return StatusCode(500, "Something went wrong when creating a city.");
                }

                // if the city request has any points of interest in the request. if so, we need to create those as well.
                if (newCityRequest.PointsOfInterest.Any())
                {
                    try
                    {
                        foreach (PointOfInterestCreateRequestDto point in newCityRequest.PointsOfInterest)
                        {
                            // create the point of interest
                            await _pointsOfInterestProcessor.CreateNewPointOfInterest(newCityDto.CityId, point);
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.LogCritical($"**** LOGGER: Exception encountered while creating a city with points of interest: {newCityRequest.Name}.", exception);
                        return StatusCode(500, "An error was encountered while creating a city with points of interest.");
                    }
                }

                // now get the complete city with any points of interest
                CityDto newCompleteCity = await _cityProcessor.GetCityById(newCityDto.CityId, true);

                // Returns 201 Created Status Code.
                // Returns the ROUTE in the RESPONSE HEADER (http://localhost:49902/api/cities/{cityId}) where you can see it.
                // pass in the name of the route, any required args as a type, and the dto to be shown in the body.
                return CreatedAtRoute("GetCityById", new { cityId = newCityDto.CityId }, newCompleteCity);
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while creating a city: {newCityRequest}.", exception);
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
        public async Task<ActionResult> BlockPostToExistingCity(Guid cityId)
        {
            // this is being a touch over-protective.  The idea is to not allow (and inform) the consumer
            // that they can post to this endpoint with an id.  Anything with an id should be done with a PUT
            // or a PATCH.
            try
            {
                bool doesCityExist = await _cityProcessor.DoesCityExist(cityId);
                if (!doesCityExist)
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
        /// also - when testing this action with the in-memory data, it will clear out the points of interest in memory.
        /// this is normal.  the actual implementation using the db will not do this.
        /// </remarks>
        /// <response code="200">returns patched city</response>
        /// <response code="404">city id not valid</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [HttpPatch("{cityId}", Name = "PatchCity")]
        public async Task<ActionResult<CityUpdateDto>> PatchCity(Guid cityId, [FromBody] JsonPatchDocument<CityUpdateDto> patchDocument)
        {
            try
            {
                if (patchDocument != null)
                {
                    if (patchDocument.Operations.Count > 0)
                    {
                        Operation operation = patchDocument.Operations[0];
                        if (operation.op == null)
                        {
                            ModelState.AddModelError("Description", "The operation is missing (replace, add, remove, etc).");
                            return BadRequest(ModelState);
                        }

                        if (operation.path == null)
                        {
                            ModelState.AddModelError("Description", "The path is missing. What do you want to update?");
                            return BadRequest(ModelState);
                        }

                        // is this a valid city?
                        bool doesCityExist = await _cityProcessor.DoesCityExist(cityId);
                        if (!doesCityExist)
                        {
                            _logger.LogInformation($"**** LOGGER: City of cityId {cityId} was not found.");
                            return NotFound($"City of cityId {cityId} was not found.");
                        }

                        // we need to map the entity to a dto so we than can map the patch to the dto and back to the entity.
                        // <casted destination type>(source).
                        var cityEntity = await _cityInfoRepository.GetCityById(cityId, false);
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

                        if (string.IsNullOrWhiteSpace(cityToPatch.Name))
                        {
                            ModelState.AddModelError("Description", "Name is missing.");
                            return BadRequest(ModelState);
                        }

                        if (string.IsNullOrWhiteSpace(cityToPatch.Description))
                        {
                            ModelState.AddModelError("Description", "Description is missing.");
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

                    ModelState.AddModelError("Description", "The patch document is not correct.");
                    return BadRequest(ModelState);
                }

                return BadRequest(ModelState);
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while trying to patch a city: {cityId}.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }
    }
}