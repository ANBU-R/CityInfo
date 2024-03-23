using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Model;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestsAsync(cityId);

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestEntity));

        }

        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterst")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterst(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }


            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterestEntity));

        }
        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest
            (int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {


            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }


            var finalPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterest);
            await _cityInfoRepository.AddPointOfInterestAsync(cityId, finalPointOfInterest);
            await _cityInfoRepository.SaveChangesAsync(); //will add id to finalPointOfInterest 

            var createdPointOfInterest = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);


            //this method setup  location header that point to url of the newly 
            //created resource
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

            /// The JsonPatchDocument is of type PointOfInterestForUpdateDto.
            /// To apply the patch, first, map the pointOfInterestEntity to a PointOfInterestForUpdateDto.
            /// Then, apply the JSON patch document to the PointOfInterestForUpdateDto.
            /// Finally, map the PointOfInterestForUpdateDto back to the pointOfInterestEntity.

            // Map the point of interest entity to a PointOfInterestForUpdateDto
            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            // Apply the patch document to the PointOfInterestForUpdateDto.
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState); // ModelState will help us to determine weathe the patch document is valid

            /// Since we're only applying the patch here, automated validation checks won't work.
            /// Therefore, we need to manually check for bad requests.

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

        /*
        [HttpDelete("{pointOfInterestId}")]
        public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = _citiesDataStore.Cities.Find(city => city.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var pointOfInterstFromStore = city.PointsOfInterest
                .FirstOrDefault(pointofInterest => pointofInterest.Id == pointOfInterestId);
            if (pointOfInterstFromStore == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfInterstFromStore);
            _mailService.Send("Point of Interest deleted", $"city id: {cityId} point of interest id: {pointOfInterestId}");

            return NoContent();
        }

        */
    }

}
