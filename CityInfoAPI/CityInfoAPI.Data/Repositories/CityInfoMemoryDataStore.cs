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
                    CityId = "38276231-1918-452d-a3e9-6f50873a95d2",
                    Name = "Chicago",
                    Description = "Home of the blues",
                    PointsOfInterest = new List<PointOfInterest>
                    {
                        new PointOfInterest{ Id = 1, CityId = "38276231-1918-452d-a3e9-6f50873a95d2", PointId = "e5a5f605-627d-4aec-9f5c-e9939ea0a6cf", Name = "Lake Michigan", Description = "Walk along the lake" },
                        new PointOfInterest { Id = 2, CityId = "38276231-1918-452d-a3e9-6f50873a95d2", PointId = "8fb872a7-2559-44b0-b89a-aeea403f58c2", Name = "Lake Docks", Description = "Rent a boat" }
                    }
                },
                new City
                {
                    Id = 2,
                    CityId = "09fdd26e-5141-416c-a590-7eaf193b9565",
                    Name = "Dallas",
                    Description = "Cowboys live here",
                    PointsOfInterest = new List<PointOfInterest>
                    {
                        new PointOfInterest { Id = 3, CityId = "09fdd26e-5141-416c-a590-7eaf193b9565", PointId = "84e3ae40-3409-4a06-aaba-b075aa4090da", Name = "Rodeo", Description = "Cowboys and horses" },
                        new PointOfInterest { Id = 4, CityId = "09fdd26e-5141-416c-a590-7eaf193b9565", PointId = "abcf9be0-d1e8-47ec-be6e-13d952907286", Name = "Steakhouse", Description = "Famous restaurant" }
                    }
                },
                new City
                {
                    Id = 3,
                    CityId = "1add03e4-d532-4811-977e-14038d7d4751",
                    Name = "New York",
                    Description = "The Big Apple",
                    PointsOfInterest = new List<PointOfInterest>
                    {
                        new PointOfInterest { Id = 5, CityId = "1add03e4-d532-4811-977e-14038d7d4751", PointId = "58be6173-a6f5-4594-8b97-c49a8b1af2d2", Name = "Central Park", Description = "This is the updated description for Central Park" },
                        new PointOfInterest { Id = 6, CityId = "1add03e4-d532-4811-977e-14038d7d4751", PointId = "65572ea5-159c-403f-acc9-ff4fd721a93f", Name = "Empire State Building", Description = "Famous landmark" },
                        new PointOfInterest { Id = 7, CityId = "1add03e4-d532-4811-977e-14038d7d4751", PointId = "1eac15dd-74f9-4adc-af14-6e6833a9dc8f", Name = "Freedom Tower", Description = "The new, shiny Freedom Tower" },
                    }
                },
                new City
                {
                    Id = 4,
                    CityId = "04074509-d937-47a2-bad1-fa3a4ec4b122",
                    Name = "Los Angeles",
                    Description = "City of Angels",
                    PointsOfInterest = new List<PointOfInterest>
                    {
                        new PointOfInterest { Id = 8, CityId = "04074509-d937-47a2-bad1-fa3a4ec4b122", PointId = "7767ff5a-b0c4-4e6a-a080-593c03b953d7", Name = "LAX", Description = "The LAX airport" },
                        new PointOfInterest { Id = 9, CityId = "04074509-d937-47a2-bad1-fa3a4ec4b122", PointId = "0b96efef-fcb5-4067-a831-56dd5ba91adb", Name = "Hollywood", Description = "Where movies are made" }
                    }
                },
                new City
                {
                    Id = 5,
                    CityId = "5c53812d-b75f-4cd5-88b6-ce06f1ab65e1",
                    Name = "Richmond",
                    Description = "Home of the politically correct",
                    PointsOfInterest = new List<PointOfInterest>
                    {
                        new PointOfInterest { Id = 10, CityId = "5c53812d-b75f-4cd5-88b6-ce06f1ab65e1", PointId = "07d8119c-2a38-4f07-a257-09d0735069f3", Name = "Kings Dominion", Description = "Good amusement park" },
                        new PointOfInterest { Id = 11, CityId = "5c53812d-b75f-4cd5-88b6-ce06f1ab65e1", PointId = "81d19a67-35a2-4d2b-91ae-c4d295af1020", Name = "Statues", Description = "A bunch of confederate statues" }
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
                    CityId = completeCity.CityId,
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

        public City GetCityById(string cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return _cities
                        .Where(c => c.CityId == cityId)
                        .FirstOrDefault();
            }
            else
            {
                // When using the in-memory data store, once you clear the list of points of interest,
                // it's cleared permanently until the app restarts.  EF uses the '.Include(). so there's no risk with using real db.
                var city = _cities.Where(c => c.CityId == cityId).FirstOrDefault();
                city.PointsOfInterest.Clear();
                return city;
            }
        }

        public bool DoesCityExist(string cityId)
        {
            return _cities.Any(c => c.CityId == cityId);
        }

        public List<PointOfInterest> GetPointsOfInterest(string cityId)
        {
            return GetCityById(cityId, true).PointsOfInterest;
        }

        public PointOfInterest GetPointOfInterestById(string cityId, string pointId)
        {
            var city = GetCityById(cityId, true);

            // pick up here:
            // determine if development branch has this issue.  The city above has points of interest.... but those points of interest have null City properties.
            // see if development does this.  I suspect that EF is having trouble making the relationship.
            // also -cross reference the Library API.
            try
            {
                var pointOfInterest = city.PointsOfInterest
                        .Where(p => p.PointId == pointId && p.City.CityId == cityId)
                        .OrderBy(p => p.Name)
                        .FirstOrDefault();
                return pointOfInterest;

            }
            catch (System.Exception exception)
            {

                throw exception;
            }

        }

        public void CreatePointOfInterest(string cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCityById(cityId, false);
            city.PointsOfInterest.Add(pointOfInterest);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            //var city = GetCityByKey(pointOfInterest.CityId, true);
            //city.PointsOfInterest.Remove(pointOfInterest);
        }

        // global
        public bool SaveChanges()
        {
            return true;
        }

    }
}
