﻿using CityInfoAPI.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CityInfoAPI.Data.Repositories
{
    public class CityInfoMemoryDataStore : ICityInfoRepository
    {
        // fields
        private List<City> _cities;

        // constructor
        public CityInfoMemoryDataStore()
        {
            _cities = new List<City>()
            {
                new City
                {
                    Id = 1,
                    CityId = new Guid("38276231-1918-452d-a3e9-6f50873a95d2"),
                    Name = "Chicago (in memory)",
                    Description = "Home of the blues",
                    CreatedOn = new DateTime(2019, 1, 1),
                    PointsOfInterest = new List<PointOfInterest>
                    {
                        new PointOfInterest{ Id = 1, CityId = new Guid("38276231-1918-452d-a3e9-6f50873a95d2"), PointId =  new Guid("e5a5f605-627d-4aec-9f5c-e9939ea0a6cf"), Name = "Lake Michigan", Description = "Walk along the lake", CreatedOn = new DateTime(2019, 1, 1) },
                        new PointOfInterest { Id = 2, CityId = new Guid("38276231-1918-452d-a3e9-6f50873a95d2"), PointId =  new Guid("8fb872a7-2559-44b0-b89a-aeea403f58c2"), Name = "Lake Docks", Description = "Rent a boat", CreatedOn = new DateTime(2019, 1, 1) }
                    }
                },
                new City
                {
                    Id = 2,
                    CityId = new Guid("09fdd26e-5141-416c-a590-7eaf193b9565"),
                    Name = "Dallas",
                    Description = "Cowboys live here",
                    CreatedOn = new DateTime(2019, 2, 1),
                    PointsOfInterest = new List<PointOfInterest>
                    {
                        new PointOfInterest { Id = 3, CityId = new Guid("09fdd26e-5141-416c-a590-7eaf193b9565"), PointId =  new Guid("84e3ae40-3409-4a06-aaba-b075aa4090da"), Name = "Rodeo", Description = "Cowboys and horses", CreatedOn = new DateTime(2019, 2, 1) },
                        new PointOfInterest { Id = 4, CityId = new Guid("09fdd26e-5141-416c-a590-7eaf193b9565"), PointId =  new Guid("abcf9be0-d1e8-47ec-be6e-13d952907286"), Name = "Steakhouse", Description = "Famous restaurant", CreatedOn = new DateTime(2019, 2, 1) }
                    }
                },
                new City
                {
                    Id = 3,
                    CityId = new Guid("1add03e4-d532-4811-977e-14038d7d4751"),
                    Name = "New York",
                    Description = "The Big Apple",
                    CreatedOn = new DateTime(2019, 3, 1),
                    PointsOfInterest = new List<PointOfInterest>
                    {
                        new PointOfInterest { Id = 5, CityId = new Guid("1add03e4-d532-4811-977e-14038d7d4751"), PointId =  new Guid("58be6173-a6f5-4594-8b97-c49a8b1af2d2"), Name = "Central Park", Description = "This is the updated description for Central Park", CreatedOn = new DateTime(2019, 3, 1) },
                        new PointOfInterest { Id = 6, CityId = new Guid("1add03e4-d532-4811-977e-14038d7d4751"), PointId =  new Guid("65572ea5-159c-403f-acc9-ff4fd721a93f"), Name = "Empire State Building", Description = "Famous landmark", CreatedOn = new DateTime(2019, 3, 1) },
                        new PointOfInterest { Id = 7, CityId = new Guid("1add03e4-d532-4811-977e-14038d7d4751"), PointId =  new Guid("1eac15dd-74f9-4adc-af14-6e6833a9dc8f"), Name = "Freedom Tower", Description = "The new, shiny Freedom Tower", CreatedOn = new DateTime(2019, 3, 1) },
                    }
                },
                new City
                {
                    Id = 4,
                    CityId = new Guid("04074509-d937-47a2-bad1-fa3a4ec4b122"),
                    Name = "Los Angeles",
                    Description = "City of Angels",
                    CreatedOn = new DateTime(2019, 4, 1),
                    PointsOfInterest = new List<PointOfInterest>
                    {
                        new PointOfInterest { Id = 8, CityId = new Guid("04074509-d937-47a2-bad1-fa3a4ec4b122"), PointId =  new Guid("7767ff5a-b0c4-4e6a-a080-593c03b953d7"), Name = "LAX", Description = "The LAX airport", CreatedOn = new DateTime(2019, 4, 1) },
                        new PointOfInterest { Id = 9, CityId = new Guid("04074509-d937-47a2-bad1-fa3a4ec4b122"), PointId =  new Guid("0b96efef-fcb5-4067-a831-56dd5ba91adb"), Name = "Hollywood", Description = "Where movies are made", CreatedOn = new DateTime(2019, 4, 1) }
                    }
                },
                new City
                {
                    Id = 5,
                    CityId = new Guid("5c53812d-b75f-4cd5-88b6-ce06f1ab65e1"),
                    Name = "Richmond",
                    Description = "Home of the politically correct",
                    CreatedOn = new DateTime(2019, 5, 1),
                    PointsOfInterest = new List<PointOfInterest>
                    {
                        new PointOfInterest { Id = 10, CityId = new Guid("5c53812d-b75f-4cd5-88b6-ce06f1ab65e1"), PointId =  new Guid("07d8119c-2a38-4f07-a257-09d0735069f3"), Name = "Kings Dominion", Description = "Good amusement park", CreatedOn = new DateTime(2019, 5, 1) },
                        new PointOfInterest { Id = 11, CityId = new Guid("5c53812d-b75f-4cd5-88b6-ce06f1ab65e1"), PointId =  new Guid("81d19a67-35a2-4d2b-91ae-c4d295af1020"), Name = "Statues", Description = "A bunch of confederate statues", CreatedOn = new DateTime(2019, 5, 1) }
                    }
                }
            };
        }

        public async Task<List<City>> GetCities()
        {
            List<City> citiesWithoutPointsOfInterest = new List<City>();
            foreach (var completeCity in _cities)
            {
                var city = new City
                {
                    Id = completeCity.Id,
                    CityId = completeCity.CityId,
                    Name = completeCity.Name,
                    Description = completeCity.Description,
                    CreatedOn = completeCity.CreatedOn,
                    PointsOfInterest = new List<PointOfInterest>()
                };
                citiesWithoutPointsOfInterest.Add(city);
            }

            return citiesWithoutPointsOfInterest.OrderBy(c => c.Name).ToList();
        }

        public void CreateCity(City city)
        {
            _cities.Add(city);
        }

        public List<City> GetCitiesWithPointsOfInterest()
        {
            return _cities.OrderBy(c => c.Name).ToList();
        }

        public City GetCityById(Guid cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                // look here.  this seems to be called multiple times
                var city =  _cities
                            .Where(c => c.CityId == cityId)
                            .FirstOrDefault();
                return city;
            }
            else
            {
                // When using the in-memory data store, once you clear the list of points of interest,
                // it's cleared permanently until the app restarts.  EF uses the '.Include(). so there's no risk with using real db.
                var city = _cities.Where(c => c.CityId == cityId).FirstOrDefault();
                if (city != null)
                {
                    city.PointsOfInterest.Clear();
                }
                return city;
            }
        }

        public bool DoesCityExist(Guid cityId)
        {
            return _cities.Any(c => c.CityId == cityId);
        }

        public List<PointOfInterest> GetPointsOfInterest(Guid cityId)
        {
            return GetCityById(cityId, true).PointsOfInterest;
        }

        public PointOfInterest GetPointOfInterestById(Guid cityId, Guid pointId)
        {
            var city = GetCityById(cityId, true);

            try
            {
                return city.PointsOfInterest
                        .Where(p => p.PointId == pointId && p.CityId == cityId)
                        .OrderBy(p => p.Name)
                        .FirstOrDefault();

            }
            catch (System.Exception exception)
            {

                throw exception;
            }

        }

        public void CreatePointOfInterest(Guid cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCityById(cityId, true);
            city.PointsOfInterest.Add(pointOfInterest);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            var city = GetCityById(pointOfInterest.CityId, true);
            city.PointsOfInterest.Remove(pointOfInterest);
        }

        // global
        public bool SaveChanges()
        {
            return true;
        }
    }
}
