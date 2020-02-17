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
                city.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities?pageNumber={defaultRequestParamaters.PageNumber}&pageSize={defaultRequestParamaters.PageSize}", "city-collection", "GET"));

                // if you wanted to expose this....then you could do this:
                // you could also wrap this in some custom logic...perhaps only show if authenticated...
                // link to create
                city.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities", "city-create", "POST"));

                // link to patch
                city.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{city.CityId}", "city-patch", "PATCH"));

            }
            catch (System.Exception exception)
            {
                throw exception;
            }
            return city;
        }

        public static LinkDto CreateLinkForCityWithinCollection(HttpRequest request, CityWithoutPointsOfInterestDto city)
        {
            string protocol = (request.IsHttps) ? "https" : "http";
            string version = "v1.0";    // we should probably look this up
            try
            {
                LinkDto link = new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{city.CityId}", "city", "GET");
                return link;
            }
            catch (System.Exception exception)
            {
                throw exception;
            }
        }

        public static PointOfInterestDto CreateLinksForPointOfInterest(HttpRequest request, PointOfInterestDto poi)
        {
            string protocol = (request.IsHttps) ? "https" : "http";
            string version = "v1.0";    // we should probably look this up
            var defaultRequestParamaters = new RequestParameters();
            try
            {
                // link to self
                poi.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{poi.CityId}/pointsofinterest/{poi.PointId}", "self", "GET"));

                // link to collection
                poi.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{poi.CityId}/pointsofinterest", "points-of-interest-collection", "GET"));

                // if you wanted to expose this....then you could do this:
                // you could also wrap this in some custom logic...perhaps only show if authenticated...
                // link to create
                poi.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{poi.CityId}/pointsofinterest", "point-of-interest-create", "POST"));

                // link to update
                poi.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{poi.CityId}/pointsofinterest/{poi.PointId}", "point-of-interest-update", "PUT"));

                // link to patch
                poi.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{poi.CityId}/pointsofinterest/{poi.PointId}", "point-of-interest-patch", "PATCH"));

                // link to delete
                poi.Links.Add(new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{poi.CityId}/pointsofinterest/{poi.PointId}", "point-of-interest-delete", "DELETE"));
            }
            catch (System.Exception exception)
            {
                throw exception;
            }
            return poi;
        }

        public static LinkDto CreateLinkForPointOfInterestWithinCollection(HttpRequest request, PointOfInterestDto poi)
        {
            string protocol = (request.IsHttps) ? "https" : "http";
            string version = "v1.0";    // we should probably look this up
            try
            {
                LinkDto link = new LinkDto($"{protocol}://{request.Host}/api/{version}/cities/{poi.CityId}/pointsofinterest/{poi.PointId}", "point-of-interest", "GET");
                return link;
            }
            catch (System.Exception exception)
            {
                throw exception;
            }
        }
    }

    #pragma warning restore CS1591

}
