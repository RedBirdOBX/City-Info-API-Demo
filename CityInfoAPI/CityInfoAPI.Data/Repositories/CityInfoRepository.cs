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

        public City GetCityById(string cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return _cityInfoDbContext.Cities
                        .Include(c => c.PointsOfInterest)
                        .Where(c => c.CityId == cityId)
                        .FirstOrDefault();
            }
            else
            {
                return _cityInfoDbContext.Cities
                        .Where(c => c.CityId == cityId)
                        .FirstOrDefault();
            }
        }

        public bool DoesCityExist(string cityId)
        {
            return _cityInfoDbContext.Cities.Any(c => c.CityId == cityId);
        }


        // points of interest
        public List<PointOfInterest> GetPointsOfInterest(string pointId)
        {
            return _cityInfoDbContext.PointsOfInterest
                    .Where(p => p.PointId == pointId)
                    .ToList();
        }

        public PointOfInterest GetPointOfInterestById(string cityId, string pointId)
        {
            return _cityInfoDbContext.PointsOfInterest
                    .Where(p => p.PointId == pointId && p.City.CityId == cityId)
                    .OrderBy(p => p.Name)
                    .FirstOrDefault();
        }

        public void CreatePointOfInterest(string cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCityById(cityId, false);
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
