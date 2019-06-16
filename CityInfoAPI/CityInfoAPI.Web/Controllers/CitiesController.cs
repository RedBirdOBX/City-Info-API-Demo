using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Logic.Processors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

        // Get all cities
        // http://{domain}/api/cities
        [HttpGet("", Name = "GetCities")]
        public IActionResult GetCities()
        {
            var results = _cityProcessor.GetCities();
            return Ok(results);
        }

        // Get city by id
        // http://{domain}/api/cities/{cityId}
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