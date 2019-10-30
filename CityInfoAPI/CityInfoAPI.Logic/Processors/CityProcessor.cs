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

        public bool DoesCityExist(string cityKey)
        {
            return _cityInfoRepository.DoesCityExist(cityKey);
        }

        public CityDto GetCityByKey(string cityKey, bool includePointsOfInterest)
        {
            var city = _cityInfoRepository.GetCityByKey(cityKey, includePointsOfInterest);
            var results = Mapper.Map<CityDto>(city);
            return results;
        }
    }
}
