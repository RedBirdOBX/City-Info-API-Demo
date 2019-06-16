using System.ComponentModel.DataAnnotations;

namespace CityInfoAPI.Dtos.Models
{
    public class PointOfInterestCreateDto
    {
        [Required(ErrorMessage = "Point of Interest Name is required.")]
        [MaxLength(50, ErrorMessage = "Point of Interest Name cannot exceed 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Point of Interest Description is required.")]
        [MaxLength(200, ErrorMessage = "Point of Interest Description cannot exceed 200 characters.")]
        public string Description { get; set; }
    }
}
