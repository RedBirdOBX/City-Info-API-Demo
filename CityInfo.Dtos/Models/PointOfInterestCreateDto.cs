using System;

namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// data is passed to the dto from the PointOfInterestCreateRequestDto
    /// this dto is responsible for creating the guid
    /// </summary>
    public class PointOfInterestCreateDto
    {
        /// <summary>
        /// constructor of dto
        /// </summary>
        public PointOfInterestCreateDto()
        {
            PointId = Guid.NewGuid();
        }

        /// <summary>
        /// key identifier of point of interest
        /// </summary>
        public Guid PointId { get; set; }

        /// <summary>
        /// key identifier of city
        /// </summary>
        public Guid CityId { get; set; }

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
