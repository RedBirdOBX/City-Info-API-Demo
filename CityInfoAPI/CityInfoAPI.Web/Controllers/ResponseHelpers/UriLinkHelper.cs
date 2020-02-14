using CityInfoAPI.Dtos.Models;
using CityInfoAPI.Web.Controllers.RequestHelpers;
using Microsoft.AspNetCore.Http;


namespace CityInfoAPI.Web.Controllers.ResponseHelpers
{

#pragma warning disable CS1591

    public static class UriLinkHelper
    {
        public static CityDto CreateLinksForCity(HttpRequest request, CityDto city)
        {
            string protocol = (request.IsHttps) ? "https" : "http";
            string version = "v1.0";    // we should probably look this up
            var defaultRequestParamaters = new RequestParameters();
            try
            {
                // link to self
                city.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{city.CityId}", "self", "GET"));

                // link to collection
                city.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities?pageNumber={defaultRequestParamaters.PageNumber}&pageSize={defaultRequestParamaters.PageSize}", "self", "GET"));
            }
            catch (System.Exception exception)
            {
                throw exception;
            }
            return city;
        }
    }

    #pragma warning restore CS1591

}
