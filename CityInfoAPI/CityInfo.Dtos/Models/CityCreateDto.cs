using System;
using System.ComponentModel.DataAnnotations;

namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// city create dto
    /// </summary>
    public class CityCreateDto
    {
        /// <summary>
        /// constructor
        /// </summary>
        public CityCreateDto()
        {
            CityId = System.Guid.NewGuid();
        }

        /// <summary>
        /// key identifier of city
        /// </summary>
        [Required(ErrorMessage = "City Id is required.")]
        public Guid CityId { get; set; }

        /// <summary>
        /// the name of the city
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the description of the city
        /// </summary>
        public string Description { get; set; }
    }
}
