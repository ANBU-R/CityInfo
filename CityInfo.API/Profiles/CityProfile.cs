using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Model;

namespace CityInfo.API.Profiles
{
    public class CityProfile : Profile
    {
        // Defines a mapping profile for AutoMapper
        // to map properties from Entities.City to Model.CityWithoutPointOfInterestDto
        public CityProfile()
        {
            CreateMap<City, CityWithoutPointOfInterestDto>();
            CreateMap<City, CityDto>();
        }

    }
}
