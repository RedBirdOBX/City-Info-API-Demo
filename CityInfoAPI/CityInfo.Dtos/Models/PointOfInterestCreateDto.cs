using System;
using System.ComponentModel.DataAnnotations;

namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// point of interest create dto
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
        [Required(ErrorMessage = "Point of Interest Id is required.")]
        public Guid PointId { get; set; }

        /// <summary>
        /// key identifier of city
        /// </summary>
        //[NotEmpty]
        //https://andrewlock.net/creating-an-empty-guid-validation-attribute/
        //https://www.c-sharpcorner.com/article/custom-data-annotation-validation-in-mvc/

        // scenario:  if the user omits the cityId in the POST, it will default to '000-000-000 yadda..' which is still a value...it's just an empty guid.
        // this should fail but doesn't.

        [Required(ErrorMessage = "City Id is required.")]
        public Guid CityId { get; set; }

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
