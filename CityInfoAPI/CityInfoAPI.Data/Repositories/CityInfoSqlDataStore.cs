using CityInfoAPI.Data.EF;
using CityInfoAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfoAPI.Data.Repositories
{
    public class CityInfoSqlDataStore : ICityInfoRepository
    {
        // fields
        private CityInfoDbContext _cityInfoDbContext;

        // constructor
        public CityInfoSqlDataStore(CityInfoDbContext cityInfoDbContext)
        {
            _cityInfoDbContext = cityInfoDbContext;
        }


        // cities
        public Task<List<City>> GetCities()
        {
            return _cityInfoDbContext.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task CreateCity(City city)
        {
            _cityInfoDbContext.Cities.Add(city);
        }

        public async Task<List<City>> GetCitiesWithPointsOfInterest()
        {
            return await _cityInfoDbContext.Cities
                    .Include(c => c.PointsOfInterest)
                    .OrderBy(c => c.Name).ToListAsync();
        }

        public Task<City> GetCityById(Guid cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return _cityInfoDbContext.Cities
                        .Include(c => c.PointsOfInterest)
                        .Where(c => c.CityId == cityId)
                        .FirstOrDefaultAsync();
            }
            else
            {
                return _cityInfoDbContext.Cities
                        .Where(c => c.CityId == cityId)
                        .FirstOrDefaultAsync();
            }
        }

        public async Task<bool> DoesCityExist(Guid cityId)
        {
            City city = await _cityInfoDbContext.Cities.Where(c => c.CityId == cityId).FirstOrDefaultAsync();
            return city != null;
        }


        // points of interest
        public Task<List<PointOfInterest>> GetPointsOfInterest(Guid cityId)
        {
            return _cityInfoDbContext.PointsOfInterest
                    .Where(p => p.CityId == cityId)
                    .ToListAsync();
        }

        public Task<PointOfInterest> GetPointOfInterestById(Guid cityId, Guid pointId)
        {
            return _cityInfoDbContext.PointsOfInterest
                    .Where(p => p.PointId == pointId && p.City.CityId == cityId)
                    .OrderBy(p => p.Name)
                    .FirstOrDefaultAsync();
        }

        public async Task CreatePointOfInterest(Guid cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityById(cityId, false);
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
