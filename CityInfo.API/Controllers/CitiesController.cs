﻿using AutoMapper;
using CityInfo.API.Model;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase

    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        private const int MAX_PAGE_SIZE = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>>
            GetCities([FromQuery] string? name, [FromQuery] string? searchQuery,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            // Ensure that the pageSize does not exceed the maximum allowed page size (MAX_PAGE_SIZE)
            pageSize = Math.Min(pageSize, MAX_PAGE_SIZE);

            // Retrieve city entities asynchronously from the city info repository
            var (cityEntities, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            /* 
               // Deprecated manual mapping logic
               // This code snippet is commented out because AutoMapper is now used for mapping

               // Initialize a list to store CityWithoutPointOfInterestDto objects
               var results = new List<CityWithoutPointOfInterestDto>();

               // Iterate through each city entity
               foreach (var cityEntity in cityEntities)
               {
                   // Create a new CityWithoutPointOfInterestDto object and map properties
                   results.Add(new CityWithoutPointOfInterestDto
                   {
                       Name = cityEntity.Name,
                       Id = cityEntity.Id,
                       Description = cityEntity.Description,
                   });
               }
            */

            // Use AutoMapper to map city entities to CityWithoutPointOfInterestDto objects and return as Ok result
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cityEntities));

        }

        [HttpGet("{id}")]


        public async Task<IActionResult> GetCity(int id, bool includePointOfInterest = false)
        {

            var cityEntity = await _cityInfoRepository.GetCityAsync(id, includePointOfInterest);
            if (cityEntity == null) return NotFound();
            return includePointOfInterest
                ? Ok(_mapper.Map<CityDto>(cityEntity))
                : Ok(_mapper.Map<CityWithoutPointOfInterestDto>(cityEntity));





        }
    }
}
