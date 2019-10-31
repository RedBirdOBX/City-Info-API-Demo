﻿namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// city with no point of interest property
    /// </summary>
    public class CityWithoutPointsOfInterestDto
    {
        /// <summary>
        /// identifier of city
        /// </summary>
        public string CityId { get; set; }

        /// <summary>
        /// city name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// city description
        /// </summary>
        public string Description { get; set; }
    }
}
