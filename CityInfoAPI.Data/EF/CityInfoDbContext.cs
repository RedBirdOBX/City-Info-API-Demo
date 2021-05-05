using CityInfoAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfoAPI.Data.EF
{
    public class CityInfoDbContext: DbContext
    {
        // constructor
        public CityInfoDbContext(DbContextOptions<CityInfoDbContext> options) : base(options)
        {
        }


        public DbSet<City> Cities { get; set; }

        public DbSet<PointOfInterest> PointsOfInterest{ get; set; }
    }
}
