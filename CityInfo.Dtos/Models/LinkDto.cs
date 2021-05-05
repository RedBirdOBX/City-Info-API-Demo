namespace CityInfoAPI.Dtos.Models
{
    /// <summary>
    /// link type to be included in responses
    /// </summary>
    public class LinkDto
    {
        /// <summary>
        /// the uri of the resource
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// the responsibility of the uri
        /// </summary>
        public string Rel { get; set; }

        /// <summary>
        /// the type of request to be made
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="href">uri to return</param>
        /// <param name="rel">responsibility of uri</param>
        /// <param name="method">type of request</param>
        public LinkDto(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}
