using AutoMapper;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfoAPI.Logic.Processors
{
    public class CityProcessor
    {
        // fields
        private ICityInfoRepository _cityInfoRepository;
        private ILogger<CityProcessor> _logger;

        // constructor
        public CityProcessor(ICityInfoRepository cityInfoRepository, ILogger<CityProcessor> logger)
        {
            _cityInfoRepository = cityInfoRepository;
            _logger = logger;
        }

        public async Task<List<CityWithoutPointsOfInterestDto>> GetAllCities()
        {
            // sometimes we need to return all cities without paging.  To check to see if it exists for example.
            var cityEntities = await _cityInfoRepository.GetCities();
            var results = Mapper.Map<List<CityWithoutPointsOfInterestDto>>(cityEntities);
            return results;
        }

        public async Task<PagedList<CityWithoutPointsOfInterestDto>> GetPagedCities(int pageNumber, int pageSize)
        {
            // to do: validate that the page number isn't too large...<-- should be done in a validator layer? //

            // 1) send ALL cities to PagedList.Create method so it can calculate
            var cityEntities = await _cityInfoRepository.GetCities();
            var citiesWithPagedCalculations = PagedList<City>.Create(cityEntities, pageNumber, pageSize);

            // 2) map them
            var cityDtos = Mapper.Map<PagedList<CityWithoutPointsOfInterestDto>>(citiesWithPagedCalculations);

            // 3) apply the slip/take
            var pagedCities = cityDtos.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return (PagedList<CityWithoutPointsOfInterestDto>)pagedCities;
        }

        public async Task<List<CityDto>> GetCitiesWithPointOfInterest()
        {
            var cities = await _cityInfoRepository.GetCitiesWithPointsOfInterest();
            var results = Mapper.Map<List<CityDto>>(cities);
            return results;
        }

        public async Task<bool> DoesCityExist(Guid cityId)
        {
            return await _cityInfoRepository.DoesCityExist(cityId);
        }

        public async Task<CityDto> GetCityById(Guid cityId, bool includePointsOfInterest)
        {
            var city = await _cityInfoRepository.GetCityById(cityId, includePointsOfInterest);
            var results = Mapper.Map<CityDto>(city);
            return results;
        }

        public async Task<CityDto> CreateCity(CityCreateDto newCityRequest)
        {
            try
            {
                var newCityEntity = Mapper.Map<CityInfoAPI.Data.Entities.City>(newCityRequest);

                // add it to memory.
                await _cityInfoRepository.CreateCity(newCityEntity);

                // save it
                bool success = _cityInfoRepository.SaveChanges();

                if (!success)
                {
                    return null;
                }

                // map new entity to dto and return it
                CityDto newCityDto = Mapper.Map<CityDto>(newCityEntity);

                return newCityDto;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error occurred when creating a city: {exception}");
                throw exception;
            }
        }
    }
}
