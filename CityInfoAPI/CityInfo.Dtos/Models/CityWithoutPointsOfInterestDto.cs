namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// city with no point of interest property
    /// </summary>
    public class CityWithoutPointsOfInterestDto
    {
        /// <summary>
        /// point of interest id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// point of interest name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// point of interest description
        /// </summary>
        public string Description { get; set; }
    }
}
