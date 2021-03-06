﻿using CityInfoAPI.Data.EF;
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
            var cities = _cityInfoDbContext.Cities.OrderBy(c => c.Name).ToListAsync();
            return cities;
        }

        public Task<List<City>> GetPagedCities(int pageNumber, int pageSize, string name, string orderNameBy)
        {
            // if using both orderByName **and** a name filter
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(orderNameBy))
            {
                if (orderNameBy.Equals("desc", StringComparison.CurrentCultureIgnoreCase))
                {
                    return _cityInfoDbContext.Cities.Where(c => c.Name.Contains(name.ToLower()))
                                                .OrderByDescending(c => c.Name)
                                                .Skip((pageNumber - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToListAsync();
                }
                else
                {
                    // orderByName had some val but was not 'desc'
                    return _cityInfoDbContext.Cities.Where(c => c.Name.Contains(name.ToLower()))
                                                .OrderBy(c => c.Name)
                                                .Skip((pageNumber - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToListAsync();
                }
            }

            // if using name filter **only**
            if (!string.IsNullOrEmpty(name))
            {
                return _cityInfoDbContext.Cities.Where(c => c.Name.Contains(name.ToLower()))
                                                .OrderBy(c => c.Name)
                                                .Skip((pageNumber - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToListAsync();
            }

            // if using order by name **only**
            if (!string.IsNullOrEmpty(orderNameBy))
            {
                if (orderNameBy.Equals("desc", StringComparison.CurrentCultureIgnoreCase))
                {
                    return _cityInfoDbContext.Cities
                                        .OrderByDescending(c => c.Name)
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();
                }
            }

            return _cityInfoDbContext.Cities.OrderBy(c => c.Name).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
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
