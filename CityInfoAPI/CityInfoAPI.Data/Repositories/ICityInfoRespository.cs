using CityInfoAPI.Data.Entities;
using System.Collections.Generic;

namespace CityInfoAPI.Data.Repositories
{
    public interface ICityInfoRepository
    {
        // cities
        IEnumerable<City> GetCities();

        City GetCityById(int id, bool includePointsOfInterest);

        bool DoesCityExist(int cityId);


        // points of interest
        IEnumerable<PointOfInterest> GetPointsOfInterest(int cityId);

        PointOfInterest GetPointOfInterestById(int cityId, int pointOfInterestId);

        void CreatePointOfInterest(int cityId, PointOfInterest pointOfInterest);

        void DeletePointOfInterest(PointOfInterest pointOfInterest);


        // global
        bool SaveChanges();
    }
}
