using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Model;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    // Indicate that this controller is an API controller
    [ApiController]
    // Define the route for accessing the points of interest endpoint, incorporating API versioning and city ID
    [Route("api/v{version:apiversion}/cities/{cityId}/pointsofinterest")]
    // Authorize access to this controller, ensuring only authenticated users can access its endpoints
    [Authorize]
    // Specify the API version of this controller
    [ApiVersion(2)]
    //All the endpoints may produce 401 code
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService localMailService, ICityInfoRepository cityInfoRepository,
             IMapper mapper
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = localMailService ?? throw new ArgumentNullException(nameof(localMailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        /// <summary>
        /// Retrieves points of interest for a given city.
        /// </summary>
        /// <param name="cityId">The ID of the city to retrieve points of interest for.</param>
        /// <returns>A list of points of interest.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            // Check if the city exists
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            // Retrieve points of interest for the specified city
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestsAsync(cityId);

            // Map the entity to DTO and return as OK response
            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestEntity));

        }

        /// <summary>
        /// Retrieves a point of interest by ID.
        /// </summary>
        /// <param name="cityId">The ID of the city.</param>
        /// <param name="pointOfInterestId">The ID of the point of interest to retrieve.</param>
        /// <returns>The point of interest.</returns>
        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterst")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterst(int cityId, int pointOfInterestId)
        {
            // Check if the city exists.
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }
            // Retrieve the point of interest entity from the repository
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointOfInterestId);

            // Check if the point of interest exists
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            // Map the point of interest entity to DTO and return as OK response
            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterestEntity));

        }
        /// <summary>
        /// Creates a new point of interest for the specified city.
        /// </summary>
        /// <param name="cityId">The ID of the city to create the point of interest for.</param>
        /// <param name="pointOfInterest">The data for the new point of interest.</param>
        /// <returns>The created point of interest.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest
            (int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {

            // Check if the city exists
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            // Map the incoming DTO to an entity
            var finalPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterest);

            // Add the point of interest to the specified city
            await _cityInfoRepository.AddPointOfInterestAsync(cityId, finalPointOfInterest);

            // Save changes to persist the newly added point of interest
            await _cityInfoRepository.SaveChangesAsync();

            // Map the created entity back to DTO
            var createdPointOfInterest = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);


            // Set up the location header that points to the URL of the newly created resource
            return CreatedAtAction("GetPointOfInterst", new
            {
                cityId,
                pointOfInterestId = createdPointOfInterest.Id

            },
            createdPointOfInterest
            );
        }


        /// <summary>
        /// Updates a point of interest for a city.
        /// </summary>
        /// <param name="cityId">The ID of the city containing the point of interest.</param>
        /// <param name="pointOfInterestId">The ID of the point of interest to update.</param>
        /// <param name="pointOfInterest">The DTO containing the updated information for the point of interest.</param>
        /// <returns>An action result indicating success or failure of the update operation.</returns>
        [HttpPut("{pointOfInterestId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId,
            PointOfInterestForUpdateDto pointOfInterest)
        {
            // Check if the city exists.
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            // Retrieve the point of interest entity from the repository.
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            // Update the values of the point of interest entity with the values from the DTO.
            _mapper.Map(pointOfInterest, pointOfInterestEntity);

            // Save changes to the database.
            await _cityInfoRepository.SaveChangesAsync();

            // Return a success response.
            return NoContent();
        }


        /// <summary>
        /// Partially updates a point of interest for a city using JSON Patch.
        /// </summary>
        /// <param name="cityId">The ID of the city containing the point of interest.</param>
        /// <param name="pointOfInterestId">The ID of the point of interest to partially update.</param>
        /// <param name="patchDocument">The JSON Patch document containing the partial updates.</param>
        /// <returns>An action result indicating success or failure of the partial update operation.</returns>
        [HttpPatch("{pointOfInterestId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PartialUpdatePointOfInterest(
            int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument
            )
        {
            // Check if the city exists.
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            // Retrieve the point of interest entity from the repository.
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            // The JsonPatchDocument is of type PointOfInterestForUpdateDto.
            // To apply the patch, first, map the pointOfInterestEntity to a PointOfInterestForUpdateDto.
            // Then, apply the JSON patch document to the PointOfInterestForUpdateDto.
            // Finally, map the PointOfInterestForUpdateDto back to the pointOfInterestEntity.

            // Map the point of interest entity to a PointOfInterestForUpdateDto
            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            // Apply the patch document to the PointOfInterestForUpdateDto.
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState); // ModelState will help us to determine weathe the patch document is valid

            // Since we're only applying the patch here, automated validation checks won't work.
            // Therefore, we need to manually check for bad requests.

            // Check if the patch document is valid.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            };

            // Manually check if the patched document passes validation rules.
            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            // Update the point of interest entity with the patched values.
            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            // Save changes to the database.
            await _cityInfoRepository.SaveChangesAsync();

            // Return a success response.
            return NoContent();
        }

        /// <summary>
        /// Deletes a point of interest for a city.
        /// </summary>
        /// <param name="cityId">The ID of the city containing the point of interest.</param>
        /// <param name="pointOfInterestId">The ID of the point of interest to delete.</param>
        /// <returns>An action result indicating success or failure of the delete operation.</returns>
        [HttpDelete("{pointOfInterestId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            // Check if the city exists.
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            // Retrieve the point of interest entity from the repository.
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            // Delete the point of interest entity.
            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            // Save changes to the database.
            await _cityInfoRepository.SaveChangesAsync();

            // Send notification email about the deletion.
            _mailService.Send("Point of Interest deleted", $"city id: {cityId} point of interest id: {pointOfInterestId}");

            // Return a success response.
            return NoContent();
        }

    }

}
