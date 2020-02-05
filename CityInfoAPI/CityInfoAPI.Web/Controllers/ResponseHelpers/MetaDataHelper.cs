using CityInfoAPI.Logic.Processors;
using CityInfoAPI.Web.Controllers.RequestHelpers;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using CityInfoAPI.Dtos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace CityInfoAPI.Web.Controllers.ResponseHelpers
{
    public static class MetaDataHelper
    {

        // to do:
        // build the previous and next urls
        // does the output of BuildCitiesMetaData really have to be a Task?  What a PITA!!


        public static PaginationMetaDataDto BuildCitiesMetaData(PagingParameters pagingParameters, CityProcessor _cityProcessor, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            var allCities = _cityProcessor.GetAllCities().Result;
            int totalPages = (int)Math.Ceiling(allCities.Count() / (double)pagingParameters.PageSize);
            int totalCount = allCities.Count();
            bool hasNextPage = pagingParameters.PageNumber < totalPages;
            bool hasPrevPage = pagingParameters.PageNumber > 1;
            string nextUrl = (pagingParameters.PageNumber < totalPages) ? CreateCitiesResourceUri(pagingParameters, ResourceUriType.NextPage, linkGenerator, httpContextAccessor) : string.Empty;
            string prevUrl = (pagingParameters.PageNumber > 1) ? CreateCitiesResourceUri(pagingParameters, ResourceUriType.PreviousPage, linkGenerator, httpContextAccessor) : string.Empty;

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

        private static string CreateCitiesResourceUri(PagingParameters pagingParameters, ResourceUriType type, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            // it the f*ing urlHelper!!
            // https://github.com/dotnet/aspnetcore/issues/5135
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-2.2#url-generation
            // https://stackoverflow.com/questions/54299423/using-linkgenerator-outside-of-controller
            try
            {
                //string results = string.Empty;

                switch (type)
                {
                    case ResourceUriType.NextPage:
                        //results = urlHelper.Link("GetPagedCities", new { pageNumber = pagingParameters.PageNumber + 1, pageSize = pagingParameters.PageSize });
                        //return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext, action: "GetPagedCities", controller: "Cities", values: new { pageNumber = pagingParameters.PageNumber + 1, pageSize = pagingParameters.PageSize  });
                        return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext, action: "GetPagedCities", controller: "Cities", values: new { pageNumber = pagingParameters.PageNumber + 1, pageSize = pagingParameters.PageSize });
                    //break;
                    case ResourceUriType.PreviousPage:
                        //results = urlHelper.Link("GetPagedCities", new { pageNumber = pagingParameters.PageNumber - 1, pageSize = pagingParameters.PageSize });
                        //return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext, action: "GetPagedCities", values: new { pageNumber = pagingParameters.PageNumber - 1, pageSize = pagingParameters.PageSize });
                        //break;
                        return "pre";
                    default:
                        //results = urlHelper.Link("GetPagedCities", new { pageNumber = pagingParameters.PageNumber, pageSize = pagingParameters.PageSize });
                        //return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext, action: "GetPagedCities", values: new { pageNumber = pagingParameters.PageNumber, pageSize = pagingParameters.PageSize });
                        //break;
                        return "default";
                }

                //return results;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

    }
}
