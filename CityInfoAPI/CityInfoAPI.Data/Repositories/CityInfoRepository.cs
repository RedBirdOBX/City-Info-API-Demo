using CityInfoAPI.Data.EF;
using CityInfoAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CityInfoAPI.Data.Repositories
{
    public class CityInfoRepository : ICityInfoRepository
    {
        // fields
        private CityInfoDbContext _cityInfoDbContext;

        // constructor
        public CityInfoRepository(CityInfoDbContext cityInfoDbContext)
        {
            _cityInfoDbContext = cityInfoDbContext;
        }


        // cities
        public List<City> GetCities()
        {
            return _cityInfoDbContext.Cities.OrderBy(c => c.Name).ToList();
        }

        public List<City> GetCitiesWithPointsOfInterest()
        {
            return _cityInfoDbContext.Cities
                    .Include(c => c.PointsOfInterest)
                    .OrderBy(c => c.Name).ToList();
        }

        public City GetCityByKey(string key, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return _cityInfoDbContext.Cities
                        .Include(c => c.PointsOfInterest)
                        .Where(c => c.Key == key)
                        .FirstOrDefault();
            }
            else
            {
                return _cityInfoDbContext.Cities
                        .Where(c => c.Key == key)
                        .FirstOrDefault();
            }
        }

        public bool DoesCityExist(string cityId)
        {
            return _cityInfoDbContext.Cities.Any(c => c.Key == cityId);
        }


        // points of interest
        public List<PointOfInterest> GetPointsOfInterest(string key)
        {
            return _cityInfoDbContext.PointsOfInterest
                    .Where(p => p.Key == key)
                    .ToList();
        }

        public PointOfInterest GetPointOfInterestById(string key, int pointOfInterestId)
        {
            return _cityInfoDbContext.PointsOfInterest
                    .Where(p => p.Id == pointOfInterestId && p.Key == key)
                    .OrderBy(p => p.Name)
                    .FirstOrDefault();
        }

        public void CreatePointOfInterest(string key, PointOfInterest pointOfInterest)
        {
            var city = GetCityByKey(key, false);
            city.PointsOfInterest.Add(pointOfInterest);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _cityInfoDbContext.PointsOfInterest.Remove(pointOfInterest);
        }

        // global
        public bool SaveChanges()
        {
            return _cityInfoDbContext.SaveChanges() >= 0;
        }
    }
}
