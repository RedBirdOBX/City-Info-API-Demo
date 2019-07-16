using AutoMapper;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using System.Collections.Generic;


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


        public bool DoesPointOfInterestExistForCity(int cityId, int pointOfInterestId)
        {
            PointOfInterest pointOfInterest = _cityInfoRepository.GetPointOfInterestById(cityId, pointOfInterestId);
            return pointOfInterest != null;
        }

        public List<PointOfInterestDto> GetPointsOfInterest(int cityId)
        {
            var pointsOfInterest = _cityInfoRepository.GetPointsOfInterest(cityId);
            var results = Mapper.Map<List<PointOfInterestDto>>(pointsOfInterest);
            return results;
        }

        public List<PointOfInterestDto> GetAllPointsOfInterest()
        {
            var pointsOfInterest = _cityInfoRepository.GetAllPointsOfInterest();
            var results = Mapper.Map<List<PointOfInterestDto>>(pointsOfInterest);
            return results;
        }

        public PointOfInterestDto GetPointOfInterestById(int cityId, int pointOfInterestId)
        {
            var pointOfInterest = _cityInfoRepository.GetPointOfInterestById(cityId, pointOfInterestId);
            var result = Mapper.Map<PointOfInterestDto>(pointOfInterest);
            return result;
        }

        public PointOfInterestDto CreateNewPointOfInterest(int cityId, PointOfInterestCreateDto submittedPointOfInterest)
        {
            // destination / source
            var newPointOfInterest = Mapper.Map<CityInfoAPI.Data.Entities.PointOfInterest>(submittedPointOfInterest);

            // add it to memory.
            _cityInfoRepository.CreatePointOfInterest(cityId, newPointOfInterest);

            bool success = _cityInfoRepository.SaveChanges();

            if (!success)
            {
                return null;
            }

            // map new entity to dto and return it
            var returnedPointOfInterest = Mapper.Map<PointOfInterestDto>(newPointOfInterest);
            return returnedPointOfInterest;
        }

        public bool UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestUpdateDto submittedPointOfInterest)
        {
            PointOfInterest entityPointOfInterest = _cityInfoRepository.GetPointOfInterestById(cityId, pointOfInterestId);

            // This is an overload. (source object >> destination object).
            // This overload will overwrite the values in the destination obj with the values in the source object.
            Mapper.Map(submittedPointOfInterest, entityPointOfInterest);
            return _cityInfoRepository.SaveChanges();
        }

        public bool DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            // get the entity
            var pointOfInterest = _cityInfoRepository.GetPointOfInterestById(cityId, pointOfInterestId);

            // all is good.  Remove the Point of Interest
            _cityInfoRepository.DeletePointOfInterest(pointOfInterest);

            return _cityInfoRepository.SaveChanges();
        }
    }
}
