using AutoMapper;
using CityInfoAPI.Dtos.Models;
using System.Collections.Generic;

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


        public List<CitySummaryDto> GetCitiesSummary()
        {
            var cities = _cityProcessor.GetCitiesWithPointOfInterest();
            var results = Mapper.Map<List<CitySummaryDto>>(cities);
            return results;
        }
    }
}
