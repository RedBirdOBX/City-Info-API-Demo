using CityInfoAPI.Logic.Processors;
using CityInfoAPI.Web.Controllers.RequestHelpers;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using CityInfoAPI.Dtos.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Web.Controllers.ResponseHelpers
{
    public static class MetaDataHelper
    {

        // to do:
        // build the previous and next urls
        // does the output of BuildCitiesMetaData really have to be a Task?  What a PITA!!


        public static PaginationMetaDataDto BuildCitiesMetaData(PagingParameters pagingParameters, CityProcessor _cityProcessor, IUrlHelper urlHelper)
        {
            var allCities = _cityProcessor.GetAllCities().Result;
            int totalPages = (int)Math.Ceiling(allCities.Count() / (double)pagingParameters.PageSize);
            int totalCount = allCities.Count();
            bool hasNextPage = pagingParameters.PageNumber < totalPages;
            bool hasPrevPage = pagingParameters.PageNumber > 1;
            string nextUrl = (pagingParameters.PageNumber < totalPages) ? CreateCitiesResourceUri(pagingParameters, ResourceUriType.NextPage, urlHelper) : string.Empty;
            string prevUrl = (pagingParameters.PageNumber > 1) ? CreateCitiesResourceUri(pagingParameters, ResourceUriType.PreviousPage, urlHelper) : string.Empty;

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

        private static string CreateCitiesResourceUri(PagingParameters pagingParameters, ResourceUriType type, IUrlHelper urlHelper)
        {
            // it the f*ing urlHelper!!
            // https://github.com/dotnet/aspnetcore/issues/5135
            try
            {
                string results = string.Empty;

                switch (type)
                {
                    case ResourceUriType.NextPage:
                        results = urlHelper.Link("GetPagedCities", new { pageNumber = pagingParameters.PageNumber + 1, pageSize = pagingParameters.PageSize });
                        break;
                    case ResourceUriType.PreviousPage:
                        results = urlHelper.Link("GetPagedCities", new { pageNumber = pagingParameters.PageNumber - 1, pageSize = pagingParameters.PageSize });
                        break;
                    default:
                        results = urlHelper.Link("GetPagedCities", new { pageNumber = pagingParameters.PageNumber, pageSize = pagingParameters.PageSize });
                        break;
                }

                return results;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

    }
}
