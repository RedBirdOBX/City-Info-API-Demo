using AutoMapper;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using System.Collections.Generic;
using System.Linq;

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


        // cities
        public List<CityWithoutPointsOfInterestDto> GetCities()
        {
            var cityEntities = _cityInfoRepository.GetCities();
            var results = Mapper.Map<List<CityWithoutPointsOfInterestDto>>(cityEntities);
            return results;
        }

        public bool DoesCityExist(int cityId)
        {
            return _cityInfoRepository.DoesCityExist(cityId);
        }

        public CityDto GetCityById(int cityId, bool includePointsOfInterest)
        {
            var city = _cityInfoRepository.GetCityById(cityId, includePointsOfInterest);
            var results = Mapper.Map<CityDto>(city);
            return results;
        }
    }
}
