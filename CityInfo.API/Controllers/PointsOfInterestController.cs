using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Model;
using CityInfo.API.Services;
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

        /*


           [HttpPatch("{pointOfInterestId}")]
           public ActionResult PartialUpdatePointOfInterest(
               int cityId, int pointOfInterestId,
               JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument
               )
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

               ///check the patch document is valid
               var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
               {
                   Name = pointOfInterstFromStore.Name,
                   Description = pointOfInterstFromStore.Description,
               };

               //apply patch  to pointOfInterestToPatch
               patchDocument.ApplyTo(pointOfInterestToPatch, ModelState); // ModelState will help us to determine weathe the patch document is valid

               //we're only applying the patch here so automated
                // validation checks won't work  so we've to check manually for bad request


               //check if patchDocument is valid (check input body have valid properties)
               if (!ModelState.IsValid)
               {
                   return BadRequest(ModelState);
               };

               // check the patched document checks out
               //  all the validation rule we put in the model like[MaxLength] [Required]
               if (!TryValidateModel(pointOfInterestToPatch))
               {
                   return BadRequest(ModelState);
               }

               //if the patchDocument is valid udate the changes 
               pointOfInterstFromStore.Name = pointOfInterestToPatch.Name;
               pointOfInterstFromStore.Description = pointOfInterestToPatch.Description;


               return NoContent();
           }


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
