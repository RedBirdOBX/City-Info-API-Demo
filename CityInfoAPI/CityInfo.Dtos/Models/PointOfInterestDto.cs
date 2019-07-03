namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// point of interest for city
    /// </summary>
    public class PointOfInterestDto
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
