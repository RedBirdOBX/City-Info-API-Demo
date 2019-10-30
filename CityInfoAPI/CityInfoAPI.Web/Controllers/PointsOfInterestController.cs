using AutoMapper;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Logic.Processors;
using CityInfoAPI.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace CityInfoAPI.Web.Controllers
{
    /// <summary>point of interest controller</summary>
    /// <example>http://{domain}/api/cities/{cityKey}</example>
    [Route("api/v{version:apiVersion}/cities/{cityKey}")]
    [ApiController]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json", "application/xml")]
    public class PointsOfInterestController: ControllerBase
    {
        // fields
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepository;
        private CityProcessor _cityProcessor;
        private PointsOfInterestProcessor _pointsOfInterestProcessor;

        /// <summary>constructor</summary>
        /// <param name="logger">logger factory to be injected</param>
        /// <param name="mailService">mail service to be injected</param>
        /// <param name="cityInfoRepository">data repository to be injected</param>
        /// <param name="cityProcessor">city processor middleware to be injected</param>
        /// <param name="pointsOfInterestProcessor">points of interest middleware to be injected</param>
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService,
            ICityInfoRepository cityInfoRepository, CityProcessor cityProcessor, PointsOfInterestProcessor pointsOfInterestProcessor)
        {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
            _cityProcessor = cityProcessor;
            _pointsOfInterestProcessor = pointsOfInterestProcessor;
        }


        /// <summary>get all points of interest for any given city</summary>
        /// <example>http://{domain}/api/v1.0/cities/{cityId}/pointsofinterest</example>
        /// <param name="cityKey">the cityKey of the city to retrieve points of interest for</param>
        /// <returns>a list of PointOfInterestDto</returns>
        /// <response code="200">returns list of points of interest for city</response>
        /// <response code="404">city id not valid</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [HttpGet("pointsofinterest")]
        public ActionResult<List<PointOfInterestDto>> GetPointsOfInterest(string cityKey)
        {
            try
            {
                if (!_cityProcessor.DoesCityExist(cityKey))
                {
                    _logger.LogInformation($"**** LOGGER: No city with the cityKey of {cityKey} was found.");
                    return NotFound($"No city with the cityKey of {cityKey} was found.");
                }
                else
                {
                    var results = _pointsOfInterestProcessor.GetPointsOfInterest(cityKey);
                    return Ok(results);
                }
            }
            catch (System.Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while looking for a point of interest with cityKey {cityKey}.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }

        /// <summary>get a point of interest by id for a city by id</summary>
        /// <example>http://{domain}/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}</example>
        /// <param name="cityKey">cityKey of city</param>
        /// <param name="pointKey">pointKey of point of interest</param>
        /// <returns>returns PointOfInterestDto</returns>
        /// <response code="200">returns point of interest for city</response>
        /// <response code="404">city id not valid</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [HttpGet("pointsofinterest/{pointKey}", Name = "GetPointOfInterestById")]
        public ActionResult<PointOfInterestDto> GetPointOfInterestById(string cityKey, string pointKey)
        {
            if (!_cityProcessor.DoesCityExist(cityKey))
            {
                _logger.LogInformation($"**** LOGGER: No city with the cityKey of {cityKey} was found.");
                return NotFound($"No city with the cityKey of {cityKey} was found.");
            }
            else
            {
                var pointOfInterest = _pointsOfInterestProcessor.GetPointOfInterestById(cityKey, pointKey);
                if (pointOfInterest == null)
                {
                    _logger.LogInformation($"**** LOGGER: No point of interest the pointKey of {pointKey} was found.");
                    return NotFound($"No point of interest with the pointKey of {pointKey} was found.");
                }
                else
                {
                    return Ok(pointOfInterest);
                }
            }
        }

        /// <summary>POST endpoint for creating a new point of interest</summary>
        /// <example>http://{domain}/api/v1.0/cities/{cityKey}/pointsofinterest</example>
        /// <param name="cityKey">required city id</param>
        /// <param name="submittedPointOfInterest">new point of interest</param>
        /// <returns>201 status code with new endpoint in header</returns>
        /// <response code="201">returns the new point of interest</response>
        /// <response code="404">city id not valid</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Consumes("application/json")]
        [HttpPost("pointsofinterest", Name = "CreatePointOfInterest")]
        public ActionResult CreatePointOfInterest(string cityKey, [FromBody] PointOfInterestCreateDto submittedPointOfInterest)
        {
            // The framework will attempt to deserialize the body post to a PointOfInterestCreateDto type.
            // If it can't, it will remain null and we know we have bad input.
            if (submittedPointOfInterest == null)
            {
                return BadRequest();
            }

            // did the submitted data meet all the rules?
            // should this be a 422?
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // is the name different than the description?
            // should this be a 422?
            if (submittedPointOfInterest.Name.ToLower().Equals(submittedPointOfInterest.Description.ToLower()))
            {
                ModelState.AddModelError("Description", "Name and Description cannot be the same.");
                return BadRequest(ModelState);
            }

            if (!_cityProcessor.DoesCityExist(cityKey))
            {
                _logger.LogInformation($"**** LOGGER: No city was found for cityKey {cityKey}.");
                return NotFound($"No city was found for cityKey {cityKey}.");
            }

            // each city can only have 25 points of interest.
            if (_pointsOfInterestProcessor.GetPointsOfInterest(cityKey).Count() >= 25)
            {
                var city = _cityProcessor.GetCityByKey(cityKey, false);
                return BadRequest($"Sorry. The city {city.Name} cannot have more that 25 points of interest.");
            }

            PointOfInterestDto newPointOfInterest = _pointsOfInterestProcessor.CreateNewPointOfInterest(cityKey, submittedPointOfInterest);

            if (newPointOfInterest == null)
            {
                return StatusCode(500, $"Something went wrong when creating a point of interest for cityKey {cityKey};");
            }
            else
            {
                // Returns 201 Created Status Code.
                // Returns the ROUTE in the RESPONSE HEADER (http://localhost:49902/api/cities/{cityId}/pointsofinterest/{newId}) where you can see it.
                return CreatedAtRoute("GetPointOfInterestById", new { cityKey = cityKey, pointKey = newPointOfInterest.Key }, newPointOfInterest);
            }
        }

        /// <summary>PUT endpoint for updating the entire point of interest</summary>
        /// <example>http://{domain}/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}</example>
        /// <param name="cityKey">required cityKey</param>
        /// <param name="pointKey">required point of interest key</param>
        /// <param name="submittedPointOfInterest">entire updated point of interest</param>
        /// <returns>Ok status with updated version of point of interest</returns>
        /// <response code="200">returns updated point of interest for city</response>
        /// <response code="404">city key not valid</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [HttpPut("pointsofinterest/{pointKey}", Name = "UpdatePointOfInterest")]
        public IActionResult UpdatePointOfInterest(string cityKey, string pointKey, [FromBody] PointOfInterestUpdateDto submittedPointOfInterest)
        {
            // The framework will attempt to deserialize the body to a PointOnInterestCreateDto. If it can't, it will remain null and we know we have bad input.
            if (submittedPointOfInterest == null)
            {
                return BadRequest();
            }

            // did the submitted data meet all the rules?
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // is the name different than the description?
            // should be 422?
            if (submittedPointOfInterest.Name.ToLower().Equals(submittedPointOfInterest.Description.ToLower()))
            {
                ModelState.AddModelError("Description", "Name and Description cannot be the same.");
                return BadRequest(ModelState);
            }

            // does the city exist?
            if (!_cityProcessor.DoesCityExist(cityKey))
            {
                _logger.LogInformation($"**** LOGGER: City of id {cityKey} was not found.");
                return NotFound("City not found");
            }

            // are the name and description provided?
            if (string.IsNullOrWhiteSpace(submittedPointOfInterest.Name) || string.IsNullOrWhiteSpace(submittedPointOfInterest.Description))
            {
                return BadRequest(ModelState);
            }

            // does point of interest exist?
            bool pointOfInterestExists = _pointsOfInterestProcessor.DoesPointOfInterestExistForCity(cityKey, pointKey);
            if (!pointOfInterestExists)
            {
                _logger.LogInformation($"**** LOGGER: An attempt was made to update a point of interest which did not exist. cityKey {cityKey}. Point Of Interest key {pointKey}.");
                return NotFound($"Point of Interest key {pointKey} was not found.");
            }

            if (!_pointsOfInterestProcessor.UpdatePointOfInterest(cityKey, pointKey, submittedPointOfInterest))
            {
                _logger.LogWarning($"**** LOGGER: An error occurred when updating the point of interest. cityKey {cityKey}. Point Of Interest key {pointKey}.");
                return StatusCode(500, "An error occurred when updating the point of interest.");
            }

            // technically, returning NoContent() is a valid response type. The client is the one who sent the data so there's no reason
            // to send them what they just sent you.  However....I like to do it just to indicate what was updated.
            return Ok(submittedPointOfInterest);
        }

        /// <summary>PATCH endpoint for updating less than the whole point of interest</summary>
        /// <example>http://{domain}/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}</example>
        /// <param name="cityKey">required city key</param>
        /// <param name="pointKey">required point of interest key</param>
        /// <param name="patchDocument">required patch document which indicates which part(s) will be updated</param>
        /// <returns>returns PointOfInterestUpdateDto with new value(s)</returns>
        /// <remarks>
        /// http://{domain}/api/{cityId}/pointsofinterest/{pointOfInterestId}
        /// ```
        /// sample patch document. allows you to patch **name and/or description**
        /// [
        ///	    {
        ///		    "op": "replace",
        ///		    "path": "/description",
        ///		    "value": "Rico's world famous restaurant."
        ///	    }
        ///]
        ///```
        /// </remarks>
        /// <response code="200">returns patched point of interest for city</response>
        /// <response code="404">city id not valid</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [HttpPatch("pointsofinterest/{pointKey}", Name = "PatchPointOfInterest")]
        public ActionResult<PointOfInterestUpdateDto> PatchPointOfInterest(string cityKey, string pointKey, [FromBody] JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {
            // see if the correct properties and type was passed in
            if (patchDocument == null)
            {
                return BadRequest();
            }

            // is this a valid city?
            if (!_cityProcessor.DoesCityExist(cityKey))
            {
                _logger.LogInformation($"**** LOGGER: City of cityKey {cityKey} was not found.");
                return NotFound($"City of cityKey {cityKey} was not found.");
            }

            // does point of interest exist?
            bool pointOfInterestExists = _pointsOfInterestProcessor.DoesPointOfInterestExistForCity(cityKey, pointKey);
            if (!pointOfInterestExists)
            {
                _logger.LogInformation($"**** LOGGER: An attempt was made to update a point of interest which did not exist. cityKey {cityKey}. Point Of Interest key {pointKey}.");
                return NotFound($"Point of Interest of key {pointKey} was not found.");
            }

            // we need to map the entity to a dto so we than can map the patch to the dto and back to the entity.
            // <casted destination type>(source).
            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestById(cityKey, pointKey);
            var pointOfInterestToPatch = Mapper.Map<PointOfInterestUpdateDto>(pointOfInterestEntity);

            // If we include the optional ModelState argument, it will send back any potential errors.
            // This is where we map new values to the properties.
            // ModelState was created here when the Model Binding was applied to the input model...the JSONPatchDocument.
            // Since the framework has no way of knowing what was required and valid in the document, it will usually have
            // no errors and be valid.
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // We can solve this with some custom logic again. are the name and description provided?
            if (string.IsNullOrWhiteSpace(pointOfInterestToPatch.Name) || string.IsNullOrWhiteSpace(pointOfInterestToPatch.Description))
            {
                return BadRequest(ModelState);
            }

            // Try to validate the model again
            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // all is good.  map the new values back to the entity
            Mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            if (!_cityInfoRepository.SaveChanges())
            {
                _logger.LogWarning("**** LOGGER: An error occurred when patching the point of interest.");
                return StatusCode(500, "An error occurred when patching the point of interest.");
            }

            return Ok(pointOfInterestToPatch);
        }

        /// <summary>DELETE operation to delete a point of interest</summary>
        /// <example>http://{domain}/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}</example>
        /// <param name="cityKey">required city id</param>
        /// <param name="pointKey">required point of interest key</param>
        /// <returns>Ok status and the name of the point of interested which was deleted</returns>
        /// <response code="200">returns the recently deleted point of interest</response>
        /// <response code="404">city id not valid</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [HttpDelete("pointsofinterest/{pointKey}")]
        public ActionResult DeletePointOfInterest(string cityKey, string pointKey)
        {
            // is this a valid city?
            if (!_cityProcessor.DoesCityExist(cityKey))
            {
                _logger.LogInformation($"**** LOGGER: City of cityKey {cityKey} was not found.");
                return NotFound($"City of cityKey {cityKey} was not found.");
            }

            // does point of interest exist?
            bool pointOfInterestExists = _pointsOfInterestProcessor.DoesPointOfInterestExistForCity(cityKey, pointKey);
            if (!pointOfInterestExists)
            {
                _logger.LogInformation($"**** LOGGER: An attempt was made to delete a point of interest which did not exist. CityId {cityKey}. Point Of Interest key {pointKey}.");
                return NotFound($"Point of Interest of key {pointKey} was not found.");
            }

            // get this one last time before it's removed so we can reference it in the response
            var pointOfInterestToBeDeleted = _pointsOfInterestProcessor.GetPointOfInterestById(cityKey, pointKey);

            if (!_pointsOfInterestProcessor.DeletePointOfInterest(cityKey, pointKey))
            {
                _logger.LogWarning("**** LOGGER: An error occurred when deleting the point of interest.");
                return StatusCode(500, "An error occurred when deleting the point of interest.");
            }

            _mailService.SendMessage("**** Point of Interest deleted", $"Point of Interest {pointOfInterestToBeDeleted.Id} {pointOfInterestToBeDeleted.Name} was deleted. ****");

            return Ok(pointOfInterestToBeDeleted.Name + " has been removed");
        }
    }
}
