using AutoMapper;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using System.Collections.Generic;

namespace CityInfoAPI.Logic.Processors
{
    public class CityProcessor
    {
        // fields
        private ICityInfoRepository _cityInfoRepository;

        // constructor
        public CityProcessor(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }


        public List<CityWithoutPointsOfInterestDto> GetCities()
        {
            var cityEntities = _cityInfoRepository.GetCities();
            var results = Mapper.Map<List<CityWithoutPointsOfInterestDto>>(cityEntities);
            return results;
        }

        public List<CityDto> GetCitiesWithPointOfInterest()
        {
            var cities = _cityInfoRepository.GetCitiesWithPointsOfInterest();
            var results = Mapper.Map<List<CityDto>>(cities);
            return results;
        }

        public bool DoesCityExist(string cityId)
        {
            return _cityInfoRepository.DoesCityExist(cityId);
        }

        public CityDto GetCityByKey(string cityId, bool includePointsOfInterest)
        {
            var city = _cityInfoRepository.GetCityById(cityId, includePointsOfInterest);
            var results = Mapper.Map<CityDto>(city);
            return results;
        }
    }
}
