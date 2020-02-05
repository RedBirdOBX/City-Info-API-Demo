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
        public static PaginationMetaDataDto BuildCitiesMetaData(PagingParameters pagingParameters, CityProcessor _cityProcessor, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            var allCities = _cityProcessor.GetAllCities().Result;
            int totalPages = (int)Math.Ceiling(allCities.Count() / (double)pagingParameters.PageSize);
            int totalCount = allCities.Count();
            bool hasNextPage = pagingParameters.PageNumber < totalPages;
            bool hasPrevPage = pagingParameters.PageNumber > 1;
            string nextUrl = (pagingParameters.PageNumber < totalPages) ? CreateCitiesResourceUri(pagingParameters, ResourceUriType.NextPage, httpContextAccessor, linkGenerator) : string.Empty;
            string prevUrl = (pagingParameters.PageNumber > 1) ? CreateCitiesResourceUri(pagingParameters, ResourceUriType.PreviousPage, httpContextAccessor, linkGenerator) : string.Empty;

            PaginationMetaDataDto results = new PaginationMetaDataDto
            {
                CurrentPage = pagingParameters.PageNumber,
                TotalPages = totalPages,
                PageSize = pagingParameters.PageSize,
                TotalCount = totalCount,
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPrevPage,
                NextPageUrl = nextUrl,
                PreviousPageUrl = prevUrl
            };

            return results;
        }

        private static string CreateCitiesResourceUri(PagingParameters pagingParameters, ResourceUriType type, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            try
            {
                switch (type)
                {
                    case ResourceUriType.NextPage:
                        return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext, action: "GetPagedCities", controller: "Cities", values: new { pageNumber = pagingParameters.PageNumber + 1, pageSize = pagingParameters.PageSize });
                    case ResourceUriType.PreviousPage:
                        return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext, action: "GetPagedCities", controller: "Cities", values: new { pageNumber = pagingParameters.PageNumber - 1, pageSize = pagingParameters.PageSize });
                    default:
                        return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext, action: "GetPagedCities", controller: "Cities", values: new { pageNumber = pagingParameters.PageNumber, pageSize = pagingParameters.PageSize });
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
