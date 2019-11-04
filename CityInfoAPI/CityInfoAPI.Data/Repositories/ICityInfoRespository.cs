using CityInfoAPI.Data.Entities;
using System;
using System.Collections.Generic;

namespace CityInfoAPI.Data.Repositories
{
    public interface ICityInfoRepository
    {
        // cities
        List<City> GetCities();

        List<City> GetCitiesWithPointsOfInterest();

        City GetCityById(Guid cityId, bool includePointsOfInterest);

        bool DoesCityExist(Guid cityId);


        // points of interest
        List<PointOfInterest> GetPointsOfInterest(Guid cityId);

        PointOfInterest GetPointOfInterestById(Guid cityId, Guid pointId);

        void CreatePointOfInterest(Guid cityId, PointOfInterest pointOfInterest);

        void DeletePointOfInterest(PointOfInterest pointOfInterest);

        // global
        bool SaveChanges();
    }
}
