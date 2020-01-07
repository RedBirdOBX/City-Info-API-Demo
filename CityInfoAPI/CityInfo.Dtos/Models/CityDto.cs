using System;
using System.Collections.Generic;

namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// city dto
    /// </summary>
    public class CityDto
    {
        /// constructor
        public CityDto()
        {
            PointsOfInterest = new List<PointOfInterestDto>();
        }

        /// <summary>
        /// identifier of city
        /// </summary>
        public Guid CityId { get; set; }

        /// <summary>
        /// the name of the city
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the description of the city
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// when record was created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// the number of points of interest the city has
        /// </summary>
        public int NumberOfPointsOfInterest
        {
            get
            {
                return PointsOfInterest.Count;
            }
        }

        /// <summary>
        /// a list of points of interest for this city
        /// </summary>
        public List<PointOfInterestDto> PointsOfInterest { get; set; }
    }
}
