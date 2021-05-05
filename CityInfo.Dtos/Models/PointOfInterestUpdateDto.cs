using System.ComponentModel.DataAnnotations;

namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// dto for updating point of interest
    /// </summary>
    public class PointOfInterestUpdateDto
    {
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
