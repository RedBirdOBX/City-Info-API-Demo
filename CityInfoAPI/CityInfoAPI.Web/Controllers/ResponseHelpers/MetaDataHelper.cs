using CityInfoAPI.Logic.Processors;
using CityInfoAPI.Web.Controllers.RequestHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;

namespace CityInfoAPI.Web.Controllers.ResponseHelpers
{
    #pragma warning disable CS1591

    public static class MetaDataHelper
    {
        public static PaginationMetaDataDto BuildCitiesMetaData(RequestParameters requestParameters, CityProcessor _cityProcessor, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            var allCities = _cityProcessor.GetAllCities().Result;
            int totalPages = (int)Math.Ceiling(allCities.Count() / (double)requestParameters.PageSize);
            int totalCount = allCities.Count();
            bool hasNextPage = requestParameters.PageNumber < totalPages;
            bool hasPrevPage = requestParameters.PageNumber > 1;
            string nextUrl = (requestParameters.PageNumber < totalPages) ? CreateCitiesResourceUri(requestParameters, ResourceUriType.NextPage, httpContextAccessor, linkGenerator) : string.Empty;
            string prevUrl = (requestParameters.PageNumber > 1) ? CreateCitiesResourceUri(requestParameters, ResourceUriType.PreviousPage, httpContextAccessor, linkGenerator) : string.Empty;

            PaginationMetaDataDto results = new PaginationMetaDataDto
            {
                CurrentPage = requestParameters.PageNumber,
                TotalPages = totalPages,
                PageSize = requestParameters.PageSize,
                TotalCount = totalCount,
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPrevPage,
                NextPageUrl = nextUrl,
                PreviousPageUrl = prevUrl
            };

            return results;
        }

        private static string CreateCitiesResourceUri(RequestParameters requestParameters, ResourceUriType type, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            try
            {
                switch (type)
                {
                    case ResourceUriType.NextPage:
                        return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext,
                                                                action: "GetPagedCities",
                                                                controller: "Cities",
                                                                values: new {
                                                                                pageNumber = requestParameters.PageNumber + 1,
                                                                                pageSize = requestParameters.PageSize ,
                                                                                name = requestParameters.Name
                                                                            });
                    case ResourceUriType.PreviousPage:
                        return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext,
                                                                action: "GetPagedCities",
                                                                controller: "Cities",
                                                                values: new {
                                                                                pageNumber = requestParameters.PageNumber - 1,
                                                                                pageSize = requestParameters.PageSize,
                                                                                name = requestParameters.Name
                                                                            });
                    default:
                        return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext,
                                                                action: "GetPagedCities",
                                                                controller: "Cities",
                                                                values: new {
                                                                                pageNumber = requestParameters.PageNumber,
                                                                                pageSize = requestParameters.PageSize,
                                                                                name = requestParameters.Name
                                                                            });
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }

    #pragma warning restore CS1591
}
