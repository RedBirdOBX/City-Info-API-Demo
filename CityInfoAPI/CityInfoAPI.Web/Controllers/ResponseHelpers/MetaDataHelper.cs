using CityInfoAPI.Logic.Processors;
using CityInfoAPI.Web.Controllers.RequestHelpers;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace CityInfoAPI.Web.Controllers.ResponseHelpers
{
    public class MetaDataHelper
    {

        // to do:
        // shouldn't the cityProcessor be injected already?  Do I really need to pass it in?
        // can I make this a statis class and method?
        // build the previous and next urls
        // does the output of BuildCitiesMetaData really have to be a Task?  What a PITA!!

        private readonly CityProcessor _cityProcessor;

        public MetaDataHelper(CityProcessor cityProcessor)
        {
            _cityProcessor = cityProcessor;
        }

        public async Task<PaginationMetaDataDto> BuildCitiesMetaData(PagingParameters pagingParameters)
        {
            var allCities = await _cityProcessor.GetAllCities();
            int totalPages = (int)Math.Ceiling(allCities.Count() / (double)pagingParameters.PageSize);

            PaginationMetaDataDto results = new PaginationMetaDataDto
            {
                CurrentPage = pagingParameters.PageNumber,
                TotalPages = totalPages,
                PageSize = pagingParameters.PageSize,
                TotalCount = allCities.Count(),
                HasNextPage = pagingParameters.PageNumber < totalPages,
                HasPreviousPage = pagingParameters.PageNumber < totalPages,
                PreviousPageUrl = "back...",
                NextPageUrl = "next..."
            };

            return results;
        }
    }
}
