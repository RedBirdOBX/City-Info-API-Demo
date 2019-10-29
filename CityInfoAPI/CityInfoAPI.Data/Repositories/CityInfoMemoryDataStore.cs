using CityInfoAPI.Data.Entities;
using System.Collections.Generic;
using System.Linq;

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
                    Name = "Chicago",
                    Description = "Home of the blues",
                    PointsOfInterest = new List<PointOfInterest>
                    {
                        new PointOfInterest{ Id = 1, CityId = 1, Name = "Lake Michigan", Description = "Walk along the lake" },
                        new PointOfInterest { Id = 2, CityId = 1, Name = "Lake Docks", Description = "Rent a boat" }
                    }
                },
                new City
                {
                    Id = 2,
                    Name = "Dallas",
                    Description = "Cowboys live here",
                    PointsOfInterest = new List<PointOfInterest>
                    {
                        new PointOfInterest { Id = 3, CityId = 2, Name = "Rodeo", Description = "Cowboys and horses" },
                        new PointOfInterest { Id = 4, CityId = 2, Name = "Steakhouse", Description = "Famous restaurant" }
                    }
                },
                new City
                {
                    Id = 3,
                    Name = "New York",
                    Description = "The Big Apple",
                    PointsOfInterest = new List<PointOfInterest>
                    {
                        new PointOfInterest { Id = 5, CityId = 3, Name = "Central Park", Description = "This is the updated description for Central Park" },
                        new PointOfInterest { Id = 6, CityId = 3, Name = "Empire State Building", Description = "Famous landmark" },
                        new PointOfInterest { Id = 7, CityId = 3, Name = "Freedom Tower", Description = "The new, shiny Freedom Tower" },
                    }
                },
                new City
                {
                    Id = 4,
                    Name = "Los Angeles",
                    Description = "City of Angels",
                    PointsOfInterest = new List<PointOfInterest>
                    {
                        new PointOfInterest { Id = 8, CityId = 4, Name = "LAX", Description = "The LAX airport" },
                        new PointOfInterest { Id = 9, CityId = 4, Name = "Hollywood", Description = "Where movies are made" }
                    }
                },
                new City
                {
                    Id = 5,
                    Name = "Richmond",
                    Description = "Home of the politically correct",
                    PointsOfInterest = new List<PointOfInterest>
                    {
                        new PointOfInterest { Id = 10, CityId = 5, Name = "Kings Dominion", Description = "Good amusement park" },
                        new PointOfInterest { Id = 11, CityId = 5, Name = "Statues", Description = "A bunch of confederate statues" }
                    }
                }
            };

        }

        public List<City> GetCities()
        {
            List<City> citiesWithoutPointsOfInterest = new List<City>();
            foreach (var completeCity in _cities)
            {
                var city = new City
                {
                    Id = completeCity.Id,
                    Name = completeCity.Name,
                    Description = completeCity.Description,
                    PointsOfInterest = new List<PointOfInterest>()
                };
                citiesWithoutPointsOfInterest.Add(city);
            }

            return citiesWithoutPointsOfInterest.OrderBy(c => c.Name).ToList();
        }

        public List<City> GetCitiesWithPointsOfInterest()
        {
            return _cities.OrderBy(c => c.Name).ToList();
        }

        public City GetCityById(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return _cities
                        .Where(c => c.Id == cityId)
                        .FirstOrDefault();
            }
            else
            {
                var city = _cities.Where(c => c.Id == cityId).FirstOrDefault();
                city.PointsOfInterest.Clear();
                return city;
            }
        }

        public bool DoesCityExist(int cityId)
        {
            return _cities.Any(c => c.Id == cityId);
        }

        public List<PointOfInterest> GetPointsOfInterest(int cityId)
        {
            return GetCityById(cityId, true).PointsOfInterest;
        }

        public PointOfInterest GetPointOfInterestById(int cityId, int pointOfInterestId)
        {
            var city = GetCityById(cityId, true);
            return city.PointsOfInterest
                    .Where(p => p.Id == pointOfInterestId && p.CityId == cityId)
                    .OrderBy(p => p.Name)
                    .FirstOrDefault();
        }

        public void CreatePointOfInterest(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCityById(cityId, false);
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
