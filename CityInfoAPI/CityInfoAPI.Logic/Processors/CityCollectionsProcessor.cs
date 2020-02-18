using AutoMapper;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CityInfoAPI.Logic.Processors
{
    public class CityCollectionsProcessor
    {
        // fields
        private ICityInfoRepository _cityInfoRepository;
        private CityProcessor _cityProcessor;

        // constructor
        public CityCollectionsProcessor(ICityInfoRepository cityInfoRepository, CityProcessor cityProcessor)
        {
            _cityInfoRepository = cityInfoRepository;
            _cityProcessor = cityProcessor;
        }

        public async Task<List<CityWithoutPointsOfInterestDto>> GetCities(string cityIds)
        {
            List<Guid> requestedGuids = new List<Guid>();
            List<CityWithoutPointsOfInterestDto> results = new List<CityWithoutPointsOfInterestDto>();

            if (!string.IsNullOrWhiteSpace(cityIds))
            {
                if (cityIds.Contains(","))
                {
                    // the user is asking for more than one
                    string[] qsIds = cityIds.Split(",");

                    foreach (string id in qsIds)
                    {
                        // parse to an actual guid
                        if (Guid.TryParse(id, out var newGuid))
                        {
                            // only add if we don't have it yet
                            if (!requestedGuids.Contains(newGuid))
                            {
                                requestedGuids.Add(newGuid);
                            }
                        }
                    }
                }
                else
                {
                    // we only received one - parse to an actual guid
                    if (Guid.TryParse(cityIds, out var newGuid))
                    {
                        requestedGuids.Add(newGuid);
                    }
                }
            }

            // now build the results
            foreach (Guid id in requestedGuids)
            {
                CityDto city = await _cityProcessor.GetCityById(id, false);
                if (city != null)
                {
                    CityWithoutPointsOfInterestDto cityForResults = new CityWithoutPointsOfInterestDto()
                    {
                        CityId = city.CityId,
                        Description = city.Description,
                        CreatedOn = city.CreatedOn,
                        Name = city.Name,
                    };

                    // add to results
                    results.Add(cityForResults);
                }
            }

            return results;
        }

        public List<CityDto> CreateCities(List<CityCreateDto> submittedCities)
        {
            // destination / source
            var newCityEntities = Mapper.Map<IEnumerable<City>>(submittedCities);

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
