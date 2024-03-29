using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Model;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    // Indicate that this controller is an API controller
    [ApiController]
    // Define the route for accessing the cities endpoint, incorporating API versioning
    [Route("api/v{version:apiversion}/cities")]
    // Authorize access to this controller, ensuring only authenticated users can access its endpoints
    [Authorize]
    // Specify that this controller supports API version 1
    [ApiVersion(1)]
    // Specify that this controller also supports API version 2
    [ApiVersion(2)]
    // All the endpoints in the controller may producce 401 unAuthorized error
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Retrieves a list of cities based on optional parameters such as name and search query.
        /// </summary>
        /// <param name="name">The name of the city to filter by.</param>
        /// <param name="searchQuery">A search query to filter cities by.</param>
        /// <param name="pageNumber">The page number of the results to retrieve.</param>
        /// <param name="pageSize">The number of cities per page.</param>
        /// <returns>A list of cities without point of interest information.</returns>
        /// <response code="200">Returns cities based in given input.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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


        /// <summary>
        /// Retrieves a city by its ID.
        /// </summary>
        /// <param name="id">The ID of the city to retrieve.</param>
        /// <param name="includePointOfInterest">A flag indicating whether to include point of interest information.</param>
        /// <returns>A city with or without PointOfInterest.</returns>
        /// <response code="200">Returns Requested City</response>  

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCity(int id, bool includePointOfInterest = false)
        {
            // Retrieves the city entity asynchronously from the city info repository
            var cityEntity = await _cityInfoRepository.GetCityAsync(id, includePointOfInterest);
            // Checks if the city entity is null, and returns NotFound if it is
            if (cityEntity == null) return NotFound();

            // Returns the city entity mapped to either CityDto or CityWithoutPointOfInterestDto based on the includePointOfInterest flag
            return includePointOfInterest
                ? Ok(_mapper.Map<CityDto>(cityEntity))
                : Ok(_mapper.Map<CityWithoutPointOfInterestDto>(cityEntity));
        }
    }
}
