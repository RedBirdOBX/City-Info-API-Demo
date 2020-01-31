using CityInfoAPI.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CityInfoAPI.Data.Repositories
{
    public interface ICityInfoRepository
    {
        // cities
        Task<List<City>> GetCities(int pageNumber, int pageSize);

        Task<List<City>> GetAllCities();

        Task<List<City>> GetCitiesWithPointsOfInterest();

        Task<City> GetCityById(Guid cityId, bool includePointsOfInterest);

        Task<bool> DoesCityExist(Guid cityId);

        Task CreateCity(City city);


        // points of interest
        Task<List<PointOfInterest>> GetPointsOfInterest(Guid cityId);

        Task<PointOfInterest> GetPointOfInterestById(Guid cityId, Guid pointId);

        Task CreatePointOfInterest(Guid cityId, PointOfInterest pointOfInterest);

        void DeletePointOfInterest(PointOfInterest pointOfInterest);

        // global
        bool SaveChanges();
    }
}
