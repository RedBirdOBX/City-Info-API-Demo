﻿using AutoMapper;
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

        public async Task<List<CityWithoutPointsOfInterestDto>> GetPagedCities(int pageNumber, int pageSize, string name, string orderNameBy)
        {
            var pagedCityEntities = await _cityInfoRepository.GetPagedCities(pageNumber, pageSize, name, orderNameBy);
            var pagedCities = Mapper.Map<List<CityWithoutPointsOfInterestDto>>(pagedCityEntities);
            return pagedCities;
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
