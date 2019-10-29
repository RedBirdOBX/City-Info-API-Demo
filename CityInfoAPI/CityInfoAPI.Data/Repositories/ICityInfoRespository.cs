using CityInfoAPI.Data.Entities;
using System.Collections.Generic;

namespace CityInfoAPI.Data.Repositories
{
    public interface ICityInfoRepository
    {
        // cities
        List<City> GetCities();

        List<City> GetCitiesWithPointsOfInterest();

        City GetCityByKey(string key, bool includePointsOfInterest);

        bool DoesCityExist(string cityId);


        // points of interest
        List<PointOfInterest> GetPointsOfInterest(string key);

        PointOfInterest GetPointOfInterestById(string key, int pointOfInterestId);

        void CreatePointOfInterest(string key, PointOfInterest pointOfInterest);

        void DeletePointOfInterest(PointOfInterest pointOfInterest);

        // global
        bool SaveChanges();
    }
}
