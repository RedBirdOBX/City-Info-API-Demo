using Newtonsoft.Json;
using System.Collections.Generic;

namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// class ti be inherited by resource dtos
    /// </summary>
    public abstract class LinkedResourcesDto
    {
        /// <summary>
        /// links to be added to resource dtos
        /// </summary>
        [JsonProperty(PropertyName = "_links")]
        public List<LinkDto> Links { get; set; }

        /// <summary>
        ///  constructor
        /// </summary>
        public LinkedResourcesDto()
        {
            Links = new List<LinkDto>();
        }
    }
}
