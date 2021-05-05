using AutoMapper;
using CityInfoAPI.Dtos.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CityInfoAPI.Logic.Processors
{
    public class ReportingProcessor
    {
        // fields
        private CityProcessor _cityProcessor;

        // constructor
        public ReportingProcessor(CityProcessor cityProcessor)
        {
            _cityProcessor = cityProcessor;
        }


        public async Task<List<CitySummaryDto>> GetCitiesSummary()
        {
            var cities = await _cityProcessor.GetCitiesWithPointOfInterest();
            var results = Mapper.Map<List<CitySummaryDto>>(cities);
            return results;
        }
    }
}
