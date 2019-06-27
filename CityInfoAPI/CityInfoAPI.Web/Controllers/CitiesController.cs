using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Logic.Processors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace CityInfoAPI.Web.Controllers
{
    // http://{domain}/api/cities
    [Route("api/cities")]
    public class CitiesController : Controller
    {
        // fields
        private ILogger<CitiesController> _logger;
        private ICityInfoRepository _cityInfoRepository;
        private CityProcessor _cityProcessor;

        // constructor
        public CitiesController(ICityInfoRepository cityInfoRepository, ILogger<CitiesController> logger, CityProcessor cityProcessor)
        {
            _cityInfoRepository = cityInfoRepository;
            _cityProcessor = cityProcessor;
            _logger = logger;
        }

        //  Get all cities
        //  http://{domain}/api/cities
        /// <summary>get a collection of all cities.</summary>
        /// <returns>a collection of all cities</returns>
        [HttpGet("", Name = "GetCities")]
        public ActionResult<List<CityDto>> GetCities()
        {
            var results = _cityProcessor.GetCities();
            return Ok(results);
        }

        // Get city by id
        // http://{domain}/api/cities/{cityId}
        /// <summary>get a specific city by id</summary>
        /// <param name="cityId">the id of the city</param>
        /// <param name="includePointsOfInterest">flag which indicates whether or not points of interest should be included.  true/false</param>
        /// <returns>a city with optional points of interest</returns>
        [HttpGet("{cityId}", Name = "GetCityById")]
        public IActionResult GetCityById([FromRoute] int cityId, [FromQuery] bool includePointsOfInterest = false)
        {
            if (!_cityProcessor.DoesCityExist(cityId))
            {
                _logger.LogInformation($"**** LOGGER: City not found using cityId {cityId}.");
                return NotFound($"City not found using cityId {cityId}.");
            }
            else
            {
                if (includePointsOfInterest)
                {
                    var city = _cityProcessor.GetCityByIdWithPointsOfInterest(cityId);
                    return Ok(city);
                }
                else
                {
                    var city = _cityProcessor.GetCityByIdWithoutPointsOfInterest(cityId);
                    return Ok(city);
                }
            }
        }
    }
}