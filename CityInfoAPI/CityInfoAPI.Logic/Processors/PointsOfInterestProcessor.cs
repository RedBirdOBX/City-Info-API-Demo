using AutoMapper;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CityInfoAPI.Logic.Processors
{
    public class PointsOfInterestProcessor
    {
        // fields
        private ICityInfoRepository _cityInfoRepository;

        // constructor
        public PointsOfInterestProcessor(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }


        public async Task<bool> DoesPointOfInterestExistForCity(Guid cityId, Guid pointId)
        {
            PointOfInterest pointOfInterest = await _cityInfoRepository.GetPointOfInterestById(cityId, pointId);
            return pointOfInterest != null;
        }

        public List<PointOfInterestDto> GetPointsOfInterest(Guid cityId)
        {
            var pointsOfInterest = _cityInfoRepository.GetPointsOfInterest(cityId);
            var results = Mapper.Map<List<PointOfInterestDto>>(pointsOfInterest);
            return results;
        }

        public PointOfInterestDto GetPointOfInterestById(Guid cityId, Guid pointId)
        {
            var pointOfInterest = _cityInfoRepository.GetPointOfInterestById(cityId, pointId);
            var result = Mapper.Map<PointOfInterestDto>(pointOfInterest);
            return result;
        }

        public PointOfInterestDto CreateNewPointOfInterest(Guid cityId, PointOfInterestCreateRequestDto newPointOfInterestRequest)
        {
            // map the request to a create dto that contains a auto-generated guid
            var newPointOfInterestDto = Mapper.Map<CityInfoAPI.Dtos.Models.PointOfInterestCreateDto>(newPointOfInterestRequest);

            // add the cityId
            newPointOfInterestDto.CityId = cityId;

            // map the completed dto to the entity
            var newPointOfInterestEntity = Mapper.Map<CityInfoAPI.Data.Entities.PointOfInterest>(newPointOfInterestDto);

            // add it to memory.
            _cityInfoRepository.CreatePointOfInterest(cityId, newPointOfInterestEntity);

            bool success = _cityInfoRepository.SaveChanges();

            if (!success)
            {
                return null;
            }

            // map new entity to dto and return it
            var returnedPointOfInterest = Mapper.Map<PointOfInterestDto>(newPointOfInterestEntity);
            return returnedPointOfInterest;
        }

        public async Task<bool> UpdatePointOfInterest(Guid cityId, Guid pointId, PointOfInterestUpdateDto submittedPointOfInterest)
        {
            PointOfInterest entityPointOfInterest = await _cityInfoRepository.GetPointOfInterestById(cityId, pointId);

            // This is an overload. (source object >> destination object).
            // This overload will overwrite the values in the destination obj with the values in the source object.
            Mapper.Map(submittedPointOfInterest, entityPointOfInterest);
            return _cityInfoRepository.SaveChanges();
        }

        public async Task<bool> DeletePointOfInterest(Guid cityId, Guid pointId)
        {
            // get the entity
            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestById(cityId, pointId);

            // all is good.  Remove the Point of Interest
            _cityInfoRepository.DeletePointOfInterest(pointOfInterest);

            return _cityInfoRepository.SaveChanges();
        }
    }
}
