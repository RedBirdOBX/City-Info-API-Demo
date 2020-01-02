﻿using AutoMapper;
using CityInfoAPI.Data.Entities;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using System;
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


        public bool DoesPointOfInterestExistForCity(Guid cityId, Guid pointId)
        {
            PointOfInterest pointOfInterest = _cityInfoRepository.GetPointOfInterestById(cityId, pointId);
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

        public PointOfInterestDto CreateNewPointOfInterest(Guid cityId, PointOfInterestCreateRequestDto submittedPointOfInterest)
        {
            // map the request to a create dto that contains a auto-generated guid
            var newPointOfInterestDto = Mapper.Map<CityInfoAPI.Dtos.Models.PointOfInterestCreateDto>(submittedPointOfInterest);

            // map the completed dto to the entity
            var newCompletePointOfInterestDto = Mapper.Map<CityInfoAPI.Data.Entities.PointOfInterest>(newPointOfInterestDto);

            // add it to memory.
            _cityInfoRepository.CreatePointOfInterest(cityId, newCompletePointOfInterestDto);

            bool success = _cityInfoRepository.SaveChanges();

            if (!success)
            {
                return null;
            }

            // map new entity to dto and return it
            var returnedPointOfInterest = Mapper.Map<PointOfInterestDto>(newCompletePointOfInterestDto);
            return returnedPointOfInterest;
        }

        public bool UpdatePointOfInterest(Guid cityId, Guid pointId, PointOfInterestUpdateDto submittedPointOfInterest)
        {
            PointOfInterest entityPointOfInterest = _cityInfoRepository.GetPointOfInterestById(cityId, pointId);

            // This is an overload. (source object >> destination object).
            // This overload will overwrite the values in the destination obj with the values in the source object.
            Mapper.Map(submittedPointOfInterest, entityPointOfInterest);
            return _cityInfoRepository.SaveChanges();
        }

        public bool DeletePointOfInterest(Guid cityId, Guid pointId)
        {
            // get the entity
            var pointOfInterest = _cityInfoRepository.GetPointOfInterestById(cityId, pointId);

            // all is good.  Remove the Point of Interest
            _cityInfoRepository.DeletePointOfInterest(pointOfInterest);

            return _cityInfoRepository.SaveChanges();
        }
    }
}
