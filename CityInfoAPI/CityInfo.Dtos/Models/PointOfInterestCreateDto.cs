using System.ComponentModel.DataAnnotations;

namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// point of interest create dto
    /// </summary>
    public class PointOfInterestCreateDto
    {

        /// <summary>
        /// key identifier of city
        /// </summary>
        [Required(ErrorMessage = "Point of Interest Key (guid) is required.")]
        [MaxLength(50, ErrorMessage = "Point of Interest Key (guid) cannot exceed 50 characters.")]
        public string Key { get; set; }

        /// <summary>
        /// point of interest name
        /// </summary>
        [Required(ErrorMessage = "Point of Interest Name is required.")]
        [MaxLength(50, ErrorMessage = "Point of Interest Name cannot exceed 50 characters.")]
        public string Name { get; set; }

        /// <summary>
        /// point of interest description
        /// </summary>
        [Required(ErrorMessage = "Point of Interest Description is required.")]
        [MaxLength(200, ErrorMessage = "Point of Interest Description cannot exceed 200 characters.")]
        public string Description { get; set; }
    }
}
