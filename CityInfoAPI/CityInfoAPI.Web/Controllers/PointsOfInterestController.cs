using AutoMapper;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Logic.Processors;
using CityInfoAPI.Web.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace CityInfoAPI.Web.Controllers
{
    // http://{domain}/api/cities/{cityId}
    [Route("api/cities/{cityId}")]
    public class PointsOfInterestController: Controller
    {
        // fields
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepository;
        private CityProcessor _cityProcessor;
        private PointsOfInterestProcessor _pointsOfInterestProcessor;

        // constructor
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService,
            ICityInfoRepository cityInfoRepository, CityProcessor cityProcessor, PointsOfInterestProcessor pointsOfInterestProcessor)
        {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
            _cityProcessor = cityProcessor;
            _pointsOfInterestProcessor = pointsOfInterestProcessor;
        }


        // http://{domain}/api/cities/{cityId}/pointsofinterest
        /// <summary>get all points of interest for any given city</summary>
        /// <param name="cityId">the id of the city to retrieve points of interest for</param>
        /// <returns>a collection of points of interest</returns>
        [HttpGet("pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                if (!_cityProcessor.DoesCityExist(cityId))
                {
                    _logger.LogInformation($"**** LOGGER: No city with the id of {cityId} was found.");
                    return NotFound($"No city with the id of {cityId} was found.");
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

        // http://{domain}/api/cities/{cityId}/pointsofinterest/{pointOfInterestId}
        /// <summary>get a point of interest by id for a city by id</summary>
        /// <param name="cityId">id of city</param>
        /// <param name="pointOfInterestId">if of point of interest</param>
        /// <returns>a single point of interest</returns>
        [HttpGet("pointsofinterest/{pointOfInterestId}", Name = "GetPointOfInterestById")]
        public IActionResult GetPointOfInterestById(int cityId, int pointOfInterestId)
        {
            if (!_cityProcessor.DoesCityExist(cityId))
            {
                _logger.LogInformation($"**** LOGGER: No city with the id of {cityId} was found.");
                return NotFound($"No city with the id of {cityId} was found.");
            }
            else
            {
                var pointOfInterest = _pointsOfInterestProcessor.GetPointOfInterestById(cityId, pointOfInterestId);
                if (pointOfInterest == null)
                {
                    _logger.LogInformation($"**** LOGGER: No point of interest the id of {pointOfInterestId} was found.");
                    return NotFound($"No point of interest with the id of {pointOfInterestId} was found.");
                }
                else
                {
                    return Ok(pointOfInterest);
                }
            }

        }

        // http://{domain}/api/cities/{cityId}/pointsofinterest
        /// <summary>POST endpoint for creating a new point of interest</summary>
        /// <param name="cityId">required city id</param>
        /// <param name="submittedPointOfInterest">new point of interest</param>
        /// <returns>201 status code with new endpoint in header</returns>
        [HttpPost("pointsofinterest", Name = "CreatePointOfInterest")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestCreateDto submittedPointOfInterest)
        {
            // The framework will attempt to deserialize the body post to a PointOfInterestCreateDto type.
            // If it can't, it will remain null and we know we have bad input.
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
            if (submittedPointOfInterest.Name.ToLower().Equals(submittedPointOfInterest.Description.ToLower()))
            {
                ModelState.AddModelError("Description", "Name and Description cannot be the same.");
                return BadRequest(ModelState);
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
                return StatusCode(500, $"Something went wrong when creating a point of interest for cityId {cityId};");
            }
            else
            {
                // once Save has been called, our new PointOfInterest has it's db generated id so we can use that now.
                // rather than expose the entire new entity, let's map it to a dto so we don't expose all of the entity.
                //var createdPointOfInterestToReturn = Mapper.Map<CityInfoAPI.Dtos.Models.PointOfInterestDto>(newPointOfInterest);

                // Returns 201 Created Status Code.
                // Returns the ROUTE in the RESPONSE HEADER (http://localhost:49902/api/cities/{cityId}/pointsofinterest/{newId}) where you can see it.
                return CreatedAtRoute("GetPointOfInterestById", new { cityId = cityId, pointOfInterestId = newPointOfInterest.Id }, newPointOfInterest);
            }
        }

        // http://{domain}/api/cities/{cityId}/pointsofinterest/{pointOfInterestId}
        /// <summary>PUT endpoint for updating the entire point of interest</summary>
        /// <param name="cityId">required city id</param>
        /// <param name="pointOfInterestId">required point of interest id</param>
        /// <param name="submittedPointOfInterest">entire updated point of interest</param>
        /// <returns>Ok status with updated version of point of interest</returns>
        [HttpPut("pointsofinterest/{pointOfInterestId}", Name = "UpdatePointOfInterest")]
        public IActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId, [FromBody] PointOfInterestUpdateDto submittedPointOfInterest)
        {
            // The framework will attempt to deserialize the body post to a PointOnInterestCreateDto type.
            // If it can't, it will remain null and we know we have bad input.
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
            bool pointOfInterestExists = _pointsOfInterestProcessor.DoesPointOfInterestExistForCity(cityId, pointOfInterestId);
            if (!pointOfInterestExists)
            {
                _logger.LogInformation($"**** LOGGER: An attempt was made to update a point of interest which did not exist. CityId {cityId}. Point Of Interest {pointOfInterestId}.");
                return NotFound($"Point of Interest of id {pointOfInterestId} was not found.");
            }

            if (!_pointsOfInterestProcessor.UpdatePointOfInterest(cityId, pointOfInterestId, submittedPointOfInterest))
            {
                _logger.LogWarning($"**** LOGGER: An error occurred when updating the point of interest. CityId {cityId}. Point Of InterestId {pointOfInterestId}.");
                return StatusCode(500, "An error occurred when updating the point of interest.");
            }

            // technically, returning NoContent() is a valid response type. The client is the one who sent the data so there's no reason
            // to send them what they just sent you.  However....I like to do it just to indicate what was updated.
            return Ok(submittedPointOfInterest);
        }

        // http://{domain}/api/{cityId}/pointsofinterest/{pointOfInterestId}
        /// <summary>PATCH endpoint for updating less than the whole point of interest</summary>
        /// <param name="cityId">required city id</param>
        /// <param name="pointOfInterestId">required point of interest id</param>
        /// <param name="patchDocument">required patch document which indicates which part(s) will be updated</param>
        /// <returns>returns Ok status with newly patched point of interest</returns>
        /// <remarks>
        /// http://{domain}/api/{cityId}/pointsofinterest/{pointOfInterestId} \
        /// sample patch document: \
        /// [ \
        ///	    { \
        ///		    "op": "replace", \
        ///		    "path": "/description", \
        ///		    "value": "Rico's world famous restaurant." \
        ///	    } \
        ///] \
        /// </remarks>
        [HttpPatch("pointsofinterest/{pointOfInterestId}", Name = "PatchPointOfInterest")]
        public IActionResult PatchPointOfInterest(int cityId, int pointOfInterestId, [FromBody] JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {
            // see if the correct properties and type was passed in
            if (patchDocument == null)
            {
                return BadRequest();
            }

            // is this a valid city?
            if (!_cityProcessor.DoesCityExist(cityId))
            {
                _logger.LogInformation($"**** LOGGER: City of id {cityId} was not found.");
                return NotFound($"City of id {cityId} was not found.");
            }

            // does point of interest exist?
            bool pointOfInterestExists = _pointsOfInterestProcessor.DoesPointOfInterestExistForCity(cityId, pointOfInterestId);
            if (!pointOfInterestExists)
            {
                _logger.LogInformation($"**** LOGGER: An attempt was made to update a point of interest which did not exist. CityId {cityId}. Point Of Interest {pointOfInterestId}.");
                return NotFound($"Point of Interest of id {pointOfInterestId} was not found.");
            }

            // we need to map the entity to a dto so we than can map the patch to the dto and back to the entity.
            // <casted destination type>(source).
            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestById(cityId, pointOfInterestId);
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

            // We can solve this with some custom logic again
            // are the name and description provided?
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

        // http://{domain}/api/{cityId}/pointsofinterest/{pointOfInterestId}
        /// <summary>DELETE operation to delete a point of interest</summary>
        /// <param name="cityId">required city id</param>
        /// <param name="pointOfInterestId">required point of interest id</param>
        /// <returns>Ok status and the name of the point of interested which was deleted</returns>
        [HttpDelete("pointsofinterest/{pointOfInterestId:int}")]
        public IActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            // is this a valid city?
            if (!_cityProcessor.DoesCityExist(cityId))
            {
                _logger.LogInformation($"**** LOGGER: City of id {cityId} was not found.");
                return NotFound($"City of id {cityId} was not found.");
            }

            // does point of interest exist?
            bool pointOfInterestExists = _pointsOfInterestProcessor.DoesPointOfInterestExistForCity(cityId, pointOfInterestId);
            if (!pointOfInterestExists)
            {
                _logger.LogInformation($"**** LOGGER: An attempt was made to delete a point of interest which did not exist. CityId {cityId}. Point Of Interest {pointOfInterestId}.");
                return NotFound($"Point of Interest of id {pointOfInterestId} was not found.");
            }

            // get this one last time before it's removed so we can reference it in the response
            var pointOfInterestToBeDeleted = _pointsOfInterestProcessor.GetPointOfInterestById(cityId, pointOfInterestId);

            if (!_pointsOfInterestProcessor.DeletePointOfInterest(cityId, pointOfInterestId))
            {
                _logger.LogWarning("**** LOGGER: An error occurred when deleting the point of interest.");
                return StatusCode(500, "An error occurred when deleting the point of interest.");
            }

            _mailService.SendMessage("**** Point of Interest deleted", $"Point of Interest {pointOfInterestToBeDeleted.Id} {pointOfInterestToBeDeleted.Name} was deleted. ****");

            return Ok(pointOfInterestToBeDeleted.Name + " has been removed");
        }
    }
}
