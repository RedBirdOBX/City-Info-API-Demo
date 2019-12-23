using AutoMapper;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Logic.Processors;
using CityInfoAPI.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CityInfoAPI.Web.Controllers
{
    /// <summary>point of interest controller</summary>
    /// <example>http://{domain}/api/v{version:apiVersion}/cities/{cityId}</example>
    [Route("api/v{version:apiVersion}/cities/{cityId}")]
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
        /// <param name="cityId">the cityId of the city to retrieve points of interest for</param>
        /// <returns>a list of PointOfInterestDto</returns>
        /// <response code="200">returns list of points of interest for city</response>
        /// <response code="404">city id not valid</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [HttpGet("pointsofinterest", Name="GetPointsOfInterest")]
        public ActionResult<List<PointOfInterestDto>> GetPointsOfInterest(Guid cityId)
        {
            try
            {
                if (!_cityProcessor.DoesCityExist(cityId))
                {
                    _logger.LogInformation($"**** LOGGER: No city with the cityId of {cityId} was found.");
                    return NotFound($"No city with the cityKey of {cityId} was found.");
                }
                else
                {
                    var results = _pointsOfInterestProcessor.GetPointsOfInterest(cityId);
                    return Ok(results);
                }
            }
            catch (System.Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while looking for a point of interest with cityId {cityId}.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }

        /// <summary>get a point of interest by id for a city by id</summary>
        /// <example>http://{domain}/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}</example>
        /// <param name="cityId">cityId of city</param>
        /// <param name="pointId">pointKey of point of interest</param>
        /// <returns>returns PointOfInterestDto</returns>
        /// <response code="200">returns point of interest for city</response>
        /// <response code="404">city id not valid</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [HttpGet("pointsofinterest/{pointId}", Name = "GetPointOfInterestById")]
        public ActionResult<PointOfInterestDto> GetPointOfInterestById(Guid cityId, Guid pointId)
        {
            try
            {
                if (!_cityProcessor.DoesCityExist(cityId))
                {
                    _logger.LogInformation($"**** LOGGER: No city with the cityId of {cityId} was found.");
                    return NotFound($"No city with the cityId of {cityId} was found.");
                }
                else
                {
                    var pointOfInterest = _pointsOfInterestProcessor.GetPointOfInterestById(cityId, pointId);
                    if (pointOfInterest == null)
                    {
                        _logger.LogInformation($"**** LOGGER: No point of interest the pointId of {pointId} was found.");
                        return NotFound($"No point of interest with the pointId of {pointId} was found.");
                    }
                    else
                    {
                        return Ok(pointOfInterest);
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while looking for a point of interest : {pointId}.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }

        /// <summary>POST endpoint for creating a new point of interest</summary>
        /// <example>http://{domain}/api/v1.0/cities/{cityId}/pointsofinterest</example>
        /// <param name="cityId">required city id</param>
        /// <param name="submittedPointOfInterest">new point of interest</param>
        /// <returns>201 status code with new endpoint in header</returns>
        /// <response code="201">returns the new point of interest</response>
        /// <response code="404">city id not valid</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Consumes("application/json")]
        [HttpPost("pointsofinterest", Name = "CreatePointOfInterest")]
        public ActionResult CreatePointOfInterest(Guid cityId, [FromBody] PointOfInterestCreateDto submittedPointOfInterest)
        {
            try
            {
                if (submittedPointOfInterest.Name.ToLower().Equals(submittedPointOfInterest.Description.ToLower()))
                {
                    ModelState.AddModelError("Description", "Name and Description cannot be the same.");
                    return BadRequest(ModelState);
                }

                // normally, we would grab the cityId out of the route and not duplicate it in the body of the post.  however, we want to
                // make it easy to map to a true entity without having to create yet another dto.  otherwise, we would have to do this:
                // create request dto (name & description) >> create point of interest dto (with cityguid) >> automapper entity.
                // the cityId can never be null. If the post excludes it, it'll simply be empty (00000000-0000-0000-0000-000000000000).
                // check for a missing/empty guid.
                if (submittedPointOfInterest.CityId == Guid.Empty)
                {
                    _logger.LogInformation($"**** LOGGER: No cityId was missing from create Point of Interest request: {submittedPointOfInterest.Name}.");
                    return BadRequest($"The city id was missing for {submittedPointOfInterest.Name}.");
                }

                if (!_cityProcessor.DoesCityExist(cityId))
                {
                    _logger.LogInformation($"**** LOGGER: No city was found for cityId {cityId}.");
                    return NotFound($"No city was found for cityId {cityId}.");
                }

                // each city can only have 25 points of interest.
                if (_pointsOfInterestProcessor.GetPointsOfInterest(cityId).Count() >= 25)
                {
                    var city = _cityProcessor.GetCityById(cityId, false);
                    return BadRequest($"Sorry. The city {city.Name} cannot have more that 25 points of interest.");
                }

                PointOfInterestDto newPointOfInterest = _pointsOfInterestProcessor.CreateNewPointOfInterest(cityId, submittedPointOfInterest);

                if (newPointOfInterest == null)
                {
                    return StatusCode(500, $"Something went wrong when creating a point of interest for cityKey {cityId};");
                }

                // Returns 201 Created Status Code.
                // Returns the ROUTE in the RESPONSE HEADER (http://localhost:49902/api/cities/{cityId}/pointsofinterest/{newId}) where you can see it.
                return CreatedAtRoute("GetPointOfInterestById", new { cityId = cityId, pointId = newPointOfInterest.PointId }, newPointOfInterest);
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while trying to create a point of interest.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }

        /// <summary>PUT endpoint for updating the entire point of interest</summary>
        /// <example>http://{domain}/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}</example>
        /// <param name="cityId">required cityId</param>
        /// <param name="pointId">required point of interest key</param>
        /// <param name="submittedPointOfInterest">entire updated point of interest</param>
        /// <returns>Ok status with updated version of point of interest</returns>
        /// <response code="200">returns updated point of interest for city</response>
        /// <response code="404">city key not valid</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [HttpPut("pointsofinterest/{pointId}", Name = "UpdatePointOfInterest")]
        public IActionResult UpdatePointOfInterest(Guid cityId, Guid pointId, [FromBody] PointOfInterestUpdateDto submittedPointOfInterest)
        {
            try
            {
                if (submittedPointOfInterest.Name.ToLower().Equals(submittedPointOfInterest.Description.ToLower()))
                {
                    ModelState.AddModelError("Description", "Name and Description cannot be the same.");
                    return BadRequest(ModelState);
                }

                // does the city exist?
                if (!_cityProcessor.DoesCityExist(cityId))
                {
                    _logger.LogInformation($"**** LOGGER: City of id {cityId} was not found.");
                    return NotFound("City not found");
                }

                // are the name and description provided?
                if (string.IsNullOrWhiteSpace(submittedPointOfInterest.Name) || string.IsNullOrWhiteSpace(submittedPointOfInterest.Description))
                {
                    return BadRequest(ModelState);
                }

                // does point of interest exist?
                bool pointOfInterestExists = _pointsOfInterestProcessor.DoesPointOfInterestExistForCity(cityId, pointId);
                if (!pointOfInterestExists)
                {
                    _logger.LogInformation($"**** LOGGER: An attempt was made to update a point of interest which did not exist. cityKey {cityId}. Point Of Interest Id {pointId}.");
                    return NotFound($"Point of Interest Id {pointId} was not found.");
                }

                if (!_pointsOfInterestProcessor.UpdatePointOfInterest(cityId, pointId, submittedPointOfInterest))
                {
                    _logger.LogWarning($"**** LOGGER: An error occurred when updating the point of interest. cityKey {cityId}. Point Of Interest Id {pointId}.");
                    return StatusCode(500, "An error occurred when updating the point of interest.");
                }

                // technically, returning NoContent() is a valid response type. The client is the one who sent the data so there's no reason
                // to send them what they just sent you.  However....I like to do it just to indicate what was updated.
                return Ok(submittedPointOfInterest);
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while trying to update a point of interest: {pointId}.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }

        /// <summary>PATCH endpoint for updating less than the whole point of interest</summary>
        /// <example>http://{domain}/api/v1.0/cities/{cityId}/pointsofinterest/{pointId}</example>
        /// <param name="cityId">required city key</param>
        /// <param name="pointId">required point of interest key</param>
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
        [HttpPatch("pointsofinterest/{pointId}", Name = "PatchPointOfInterest")]
        public ActionResult<PointOfInterestUpdateDto> PatchPointOfInterest(Guid cityId, Guid pointId, [FromBody] JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {
            try
            {
                // problem: is we send a malformed patch document, it still will not be null?  Even worse, the final response code of 200
                // will be returned...which is not correct.
                // how do we ensure a VALID patch document is sent in?



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

                // does point of interest exist?
                bool pointOfInterestExists = _pointsOfInterestProcessor.DoesPointOfInterestExistForCity(cityId, pointId);
                if (!pointOfInterestExists)
                {
                    _logger.LogInformation($"**** LOGGER: An attempt was made to update a point of interest which did not exist. cityKey {cityId}. Point Of Interest Id {pointId}.");
                    return NotFound($"Point of Interest of Id {pointId} was not found.");
                }

                // we need to map the entity to a dto so we than can map the patch to the dto and back to the entity.
                // <casted destination type>(source).
                var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestById(cityId, pointId);
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
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while trying to patch a point of interest : {pointId}.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }

        /// <summary>DELETE operation to delete a point of interest</summary>
        /// <example>http://{domain}/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}</example>
        /// <param name="cityId">required city id</param>
        /// <param name="pointId">required point of interest Id</param>
        /// <returns>Ok status and the name of the point of interested which was deleted</returns>
        /// <response code="200">returns the recently deleted point of interest</response>
        /// <response code="404">city id not valid</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [HttpDelete("pointsofinterest/{pointId}", Name = "DeletePointOfInterest")]
        public ActionResult DeletePointOfInterest(Guid cityId, Guid pointId)
        {
            try
            {
                // is this a valid city?
                if (!_cityProcessor.DoesCityExist(cityId))
                {
                    _logger.LogInformation($"**** LOGGER: City of cityKey {cityId} was not found.");
                    return NotFound($"City of cityKey {cityId} was not found.");
                }

                // does point of interest exist?
                bool pointOfInterestExists = _pointsOfInterestProcessor.DoesPointOfInterestExistForCity(cityId, pointId);
                if (!pointOfInterestExists)
                {
                    _logger.LogInformation($"**** LOGGER: An attempt was made to delete a point of interest which did not exist. CityId {cityId}. Point Of Interest key {pointId}.");
                    return NotFound($"Point of Interest of key {pointId} was not found.");
                }

                // get this one last time before it's removed so we can reference it in the response
                var pointOfInterestToBeDeleted = _pointsOfInterestProcessor.GetPointOfInterestById(cityId, pointId);

                if (!_pointsOfInterestProcessor.DeletePointOfInterest(cityId, pointId))
                {
                    _logger.LogWarning("**** LOGGER: An error occurred when deleting the point of interest.");
                    return StatusCode(500, "An error occurred when deleting the point of interest.");
                }

                _mailService.SendMessage("**** Point of Interest deleted", $"Point of Interest {pointOfInterestToBeDeleted.PointId} {pointOfInterestToBeDeleted.Name} was deleted. ****");

                return Ok(pointOfInterestToBeDeleted.Name + " has been removed");
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while trying to delete a point of interest: {pointId}.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }

        /// <summary>blocks a post to a point of interest that already exists</summary>
        /// <example>http://{domain}/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}</example>
        /// <param name="cityId">id of city</param>
        /// <param name="pointOfInterestId">id of point of interest</param>
        /// <returns>status codes or bad request</returns>
        /// <response code="409">warning - cannot post with id</response>
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesDefaultResponseType]
        [HttpPost("pointsofinterest/{pointOfInterestId}", Name = "BlockPostToExistingPointOfInterest")]
        public ActionResult BlockPostToExistingPointOfInterest(Guid cityId, Guid pointOfInterestId)
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

                if (!_pointsOfInterestProcessor.DoesPointOfInterestExistForCity(cityId, pointOfInterestId))
                {
                    return BadRequest("You cannot post to point of interest like this.");
                }
                else
                {
                    return StatusCode(409, "You cannot post to an point of interest city!");
                }

            }
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while posting to BlockPostToExistingPointOfInterest: {pointOfInterestId}.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }
    }
}
