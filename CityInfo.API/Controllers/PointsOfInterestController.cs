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
        private readonly CitiesDataStore _citiesDataStore;
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService localMailService, CitiesDataStore citiesDataStore
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = localMailService ?? throw new ArgumentNullException(nameof(localMailService));
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        }

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            var city = _citiesDataStore.Cities.Find(city => city.Id == cityId);
            if (city == null)
            {

                return NotFound();
            }

            return Ok(city.PointsOfInterest);

        }

        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterst")]
        public ActionResult<PointOfInterestDto> GetPointOfInterst(int cityId, int pointOfInterestId
            )
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(city => city.Id == cityId);
            if (city == null)
            {
                _logger.LogInformation($"city with id {cityId} not found");
                return NotFound();
            }
            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (pointOfInterest == null)
            {
                return NotFound();
            }
            return Ok(pointOfInterest);

        }
        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest
            (int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {

            var city = _citiesDataStore.Cities.Find(city => city.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var maxId = _citiesDataStore.Cities
                .SelectMany(city => city.PointsOfInterest)
                .Max(p => p.Id);
            var finalPointOfInterest = new PointOfInterestDto
            {
                Id = ++maxId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description,

            };
            city.PointsOfInterest.Add(finalPointOfInterest);

            //this method setup  location header that point to url of the newly 
            //created resource
            return CreatedAtAction("GetPointOfInterst", new
            {
                cityId,
                pointOfInterestId = finalPointOfInterest.Id

            },
            finalPointOfInterest
            );
        }
        [HttpPut("{pointOfInterestId}")]
        public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId,
            PointOfInterestForUpdateDto pointOfInterest)
        {
            var city = _citiesDataStore.Cities.Find(city => city.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var pointOfInterstFromStore = city.PointsOfInterest.FirstOrDefault(pointofInterest => pointofInterest.Id == pointOfInterestId);
            if (pointOfInterstFromStore == null)
            {
                return NotFound();
            }

            pointOfInterstFromStore.Name = pointOfInterest.Name;
            pointOfInterstFromStore.Description = pointOfInterest.Description;
            return NoContent();
        }

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

            /*we're only applying the patch here so automated
             * validation checks won't work  so we've to check manually for bad request
             */

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
    }
}
