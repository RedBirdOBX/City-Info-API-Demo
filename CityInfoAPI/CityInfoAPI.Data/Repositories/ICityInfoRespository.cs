using CityInfoAPI.Data.Entities;
using System.Collections.Generic;

namespace CityInfoAPI.Data.Repositories
{
    public interface ICityInfoRepository
    {
        // cities
        List<City> GetCities();

        List<City> GetCitiesWithPointsOfInterest();

        City GetCityById(string cityId, bool includePointsOfInterest);

        bool DoesCityExist(string cityId);


        // points of interest
        List<PointOfInterest> GetPointsOfInterest(string key);

        PointOfInterest GetPointOfInterestById(string cityId, string pointId);

        void CreatePointOfInterest(string cityId, PointOfInterest pointOfInterest);

        void DeletePointOfInterest(PointOfInterest pointOfInterest);

        // global
        bool SaveChanges();
    }
}
