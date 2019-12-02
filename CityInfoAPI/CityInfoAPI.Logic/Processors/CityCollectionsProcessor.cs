using AutoMapper;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using System;
using System.Collections.Generic;

namespace CityInfoAPI.Logic.Processors
{
    public class CityCollectionsProcessor
    {
        // fields
        private ICityInfoRepository _cityInfoRepository;

        // constructor
        public CityCollectionsProcessor(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }


        public List<CityDto> CreateCities(List<CityCreateDto> cities)
        {
            // destination / source
            var newCityEntities = Mapper.Map<IEnumerable<City>>(cities);

            bool success = false;
            foreach (var newCity in newCityEntities)
            {
                _cityInfoRepository.CreateCity(newCity);
                success = _cityInfoRepository.SaveChanges();

                // if something went wrong with one or more save, get out.
                if (!success)
                {
                    return null;
                }
            }

            // map dtos to new entities and return
            var returnedCities = Mapper.Map<List<CityDto>>(newCityEntities);
            return returnedCities;
        }
    }
}
