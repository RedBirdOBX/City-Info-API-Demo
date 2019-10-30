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

        PointOfInterest GetPointOfInterestById(string cityKey, string pointKey);

        void CreatePointOfInterest(string cityKey, PointOfInterest pointOfInterest);

        void DeletePointOfInterest(PointOfInterest pointOfInterest);

        // global
        bool SaveChanges();
    }
}
