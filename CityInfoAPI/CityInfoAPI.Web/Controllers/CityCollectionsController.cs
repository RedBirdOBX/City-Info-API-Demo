using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Logic.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CityInfoAPI.Web.Controllers
{
    [Route("api/v{version:apiVersion}/citycollections")]
    [ApiController]
    public class CityCollectionsController : ControllerBase
    {

        // fields
        private ILogger<CitiesController> _logger;
        private CityCollectionsProcessor _cityCollectionsProcessor;
        private CityProcessor _cityProcessor;

        /// <summary>
        ///
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cityCollectionsProcessor"></param>
        public CityCollectionsController(ILogger<CitiesController> logger, CityCollectionsProcessor cityCollectionsProcessor, CityProcessor cityProcessor)
        {
            _logger = logger;
            _cityCollectionsProcessor = cityCollectionsProcessor;
            _cityProcessor = cityProcessor;
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="cityIds">comma delimited list of city ids (guids)</param>
        /// <returns></returns>
        /// http://localhost:5000/api/v1.0/citycollections?cityIds=38276231-1918-452d-a3e9-6f50873a95d2,09fdd26e-5141-416c-a590-7eaf193b9565,09fdd26e-5141-416c-a590-7eaf193b9565
        [HttpGet("", Name ="GetCitiesById")]
        public ActionResult<List<CityDto>> GetCitiesById([FromQuery] string cityIds)
        {
            // to do:
            // catch invalid guids and prevent exceptions
            // eliminate dups in results

            List<Guid> requestedGuids = new List<Guid>();
            List<CityDto> results = new List<CityDto>();

            try
            {
                if (!string.IsNullOrWhiteSpace(cityIds))
                {
                    if (cityIds.Contains(","))
                    {
                        // do we have more than one?
                        string[] qsIds = cityIds.Split(",");

                        // convert each to a guid
                        // perhaps shorten this w/ some linq
                        foreach (string id in qsIds)
                        {
                            Guid newGuid = Guid.Parse(id);
                            requestedGuids.Add(newGuid);
                        }
                    }
                    else
                    {
                        // we only received one
                        Guid newGuid = Guid.Parse(cityIds);
                        requestedGuids.Add(newGuid);
                    }
                }

                // now build the results
                foreach (Guid id in requestedGuids)
                {
                    CityDto city = _cityProcessor.GetCityById(id, false);

                    if (city == null)
                    {
                        // there was a bad guid sent
                        _logger.LogInformation($"**** LOGGER: City not found using cityId {id}.");
                        return NotFound($"City not found using cityId {id}.");
                    }

                    // add to results
                    results.Add(city);
                }

                return results;
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"**** LOGGER: Exception encountered while looking for a cities: {cityIds}.", exception);
                return StatusCode(500, "A problem was encountered while processing your request.");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cities"></param>
        /// <returns></returns>
        [HttpPost("", Name = "CreateCities")]
        public ActionResult<List<CityDto>> CreateCityCollections([FromBody] List<CityCreateDto> cities)
        {
            if (cities == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                List<CityDto> newCities = _cityCollectionsProcessor.CreateCities(cities);

                if (newCities == null)
                {
                    return StatusCode(500, "Something went wrong when creating a collection of cities.");
                }
                else
                {
                    // Returns 201 Created Status Code.
                    // Returns the ROUTE in the RESPONSE HEADER (http://localhost:49902/api/cities/{cityId}) where you can see it.
                    // pass in the name of the route, any required args as a type, and the dto to be shown in the body.
                    //return CreatedAtRoute("GetCitiesById", new { cityId = newCityDto.CityId }, newCityDto);
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}