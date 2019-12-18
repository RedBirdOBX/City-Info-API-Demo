using System.ComponentModel.DataAnnotations;

namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// city update dto
    /// </summary>
    public class CityUpdateDto
    {
        /// <summary>
        /// the name of the city
        /// </summary>
        [Required(ErrorMessage = "City Name is required.")]
        [MaxLength(50, ErrorMessage = "City Name cannot exceed 50 characters.")]
        public string Name { get; set; }

        /// <summary>
        /// the description of the city
        /// </summary>
        [Required(ErrorMessage = "City Description is required.")]
        [MaxLength(200, ErrorMessage = "City Description cannot exceed 200 characters.")]
        public string Description { get; set; }
    }
}
