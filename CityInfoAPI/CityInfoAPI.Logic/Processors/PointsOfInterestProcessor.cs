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


        public bool DoesPointOfInterestExistForCity(string cityKey, string pointKey)
        {
            PointOfInterest pointOfInterest = _cityInfoRepository.GetPointOfInterestById(cityKey, pointKey);
            return pointOfInterest != null;
        }

        public List<PointOfInterestDto> GetPointsOfInterest(string cityKey)
        {
            var pointsOfInterest = _cityInfoRepository.GetPointsOfInterest(cityKey);
            var results = Mapper.Map<List<PointOfInterestDto>>(pointsOfInterest);
            return results;
        }

        public PointOfInterestDto GetPointOfInterestById(string cityKey, string pointKey)
        {
            var pointOfInterest = _cityInfoRepository.GetPointOfInterestById(cityKey, pointKey);
            var result = Mapper.Map<PointOfInterestDto>(pointOfInterest);
            return result;
        }

        public PointOfInterestDto CreateNewPointOfInterest(string cityKey, PointOfInterestCreateDto submittedPointOfInterest)
        {
            // destination / source
            var newPointOfInterest = Mapper.Map<CityInfoAPI.Data.Entities.PointOfInterest>(submittedPointOfInterest);

            // add it to memory.
            _cityInfoRepository.CreatePointOfInterest(cityKey, newPointOfInterest);

            bool success = _cityInfoRepository.SaveChanges();

            if (!success)
            {
                return null;
            }

            // map new entity to dto and return it
            var returnedPointOfInterest = Mapper.Map<PointOfInterestDto>(newPointOfInterest);
            return returnedPointOfInterest;
        }

        public bool UpdatePointOfInterest(string cityKey, string pointKey, PointOfInterestUpdateDto submittedPointOfInterest)
        {
            PointOfInterest entityPointOfInterest = _cityInfoRepository.GetPointOfInterestById(cityKey, pointKey);

            // This is an overload. (source object >> destination object).
            // This overload will overwrite the values in the destination obj with the values in the source object.
            Mapper.Map(submittedPointOfInterest, entityPointOfInterest);
            return _cityInfoRepository.SaveChanges();
        }

        public bool DeletePointOfInterest(string cityKey, string pointKey)
        {
            // get the entity
            var pointOfInterest = _cityInfoRepository.GetPointOfInterestById(cityKey, pointKey);

            // all is good.  Remove the Point of Interest
            _cityInfoRepository.DeletePointOfInterest(pointOfInterest);

            return _cityInfoRepository.SaveChanges();
        }
    }
}
