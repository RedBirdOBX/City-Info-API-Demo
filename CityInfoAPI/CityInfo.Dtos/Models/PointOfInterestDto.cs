namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// point of interest for city
    /// </summary>
    public class PointOfInterestDto
    {
        /// <summary>
        /// key identifier of point of interest
        /// </summary>
        public string PointId { get; set; }

        /// <summary>
        /// point of interest name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// point of interest description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// id of city this point of interest belongs to
        /// </summary>
        public string CityId { get; set; }
    }
}
