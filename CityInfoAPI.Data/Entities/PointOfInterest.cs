using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfoAPI.Data.Entities
{
    public class PointOfInterest
    {
        public PointOfInterest()
        {
            CreatedOn = DateTime.Now;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key]
        [Required(ErrorMessage = "PointId is required.")]
        [MaxLength(50, ErrorMessage = "PointId cannot exceed 50 characters.")]
        public Guid PointId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string Description { get; set; }

        public Guid CityId { get; set; }

        [ForeignKey("CityId")]
        public City City { get; set; }

        [Required(ErrorMessage = "CreatedOn is required.")]
        public DateTime CreatedOn { get; set; }
    }
}
