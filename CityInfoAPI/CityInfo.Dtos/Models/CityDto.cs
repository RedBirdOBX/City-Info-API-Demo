using System.Collections.Generic;

namespace CityInfoAPI.Dtos.Models
{
    public class CityDto
    {
        // constructor
        public CityDto()
        {
            PointsOfInterest = new List<PointOfInterestDto>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int NumberOfPointsOfInterest
        {
            get
            {
                return PointsOfInterest.Count;
            }
        }

        public List<PointOfInterestDto> PointsOfInterest { get; set; }
    }
}
