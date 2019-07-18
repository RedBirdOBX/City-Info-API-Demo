namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// dto to provide city name and count of points of interest
    /// </summary>
    public class CitySummaryDto
    {
        /// <summary>
        /// name of city
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// total number of points of interest for city
        /// </summary>
        public int NumberOfPointsOfInterest { get; set; }
    }
}
