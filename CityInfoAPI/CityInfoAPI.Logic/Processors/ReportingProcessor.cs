using AutoMapper;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Dtos.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CityInfoAPI.Logic.Processors
{
    public class ReportingProcessor
    {
        // fields
        private ICityInfoRepository _cityInfoRepository;

        // constructor
        public ReportingProcessor(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }


        public List<CitySummaryDto> GetCitiesSummary()
        {
            // 1 - get cities from processor, not repository.  This will automatically have the name and the count of points of interest,.
            // 2 - map that to the CitySummaryDto

            //var cities = _cityInfoRepository.GetCities();
            //var results = Mapper.Map<List<CitySummaryDto>>(cityEntities);
            //return results;
        }


    }
}
