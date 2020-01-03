﻿using AutoMapper;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

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

        public bool DoesCityExist(Guid cityId)
        {
            return _cityInfoRepository.DoesCityExist(cityId);
        }

        public CityDto GetCityById(Guid cityId, bool includePointsOfInterest)
        {
            var city = _cityInfoRepository.GetCityById(cityId, includePointsOfInterest);
            var results = Mapper.Map<CityDto>(city);
            return results;
        }

        public CityDto CreateCity(CityCreateDto newCityRequest)
        {
            try
            {
                var newCityEntity = Mapper.Map<CityInfoAPI.Data.Entities.City>(newCityRequest);

                // add it to memory.
                _cityInfoRepository.CreateCity(newCityEntity);

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
