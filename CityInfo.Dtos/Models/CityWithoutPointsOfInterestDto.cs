using System;

namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// city with no point of interest property
    /// </summary>
    public class CityWithoutPointsOfInterestDto : LinkedResourcesDto
    {
        /// <summary>
        /// identifier of city
        /// </summary>
        public Guid CityId { get; set; }

        /// <summary>
        /// city name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// city description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// when record was created
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}
