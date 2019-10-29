using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfoAPI.Data.Entities
{
    public class City
    {
        // constructor
        public City()
        {
            PointsOfInterest = new List<PointOfInterest>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Key (guid) is required.")]
        [MaxLength(50, ErrorMessage = "Key (guid) cannot exceed 50 characters.")]
        public string Key { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string Description { get; set; }

        public List<PointOfInterest> PointsOfInterest { get; set;  }
    }
}
