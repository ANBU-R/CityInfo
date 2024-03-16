using CityInfo.API.Model;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.Find(city => city.Id == cityId);
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
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);
            if (city == null)
            {
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

            var city = CitiesDataStore.Current.Cities.Find(city => city.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var maxId = CitiesDataStore.Current.Cities
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
        public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        {
            var city = CitiesDataStore.Current.Cities.Find(city => city.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var pointOfInterstInStore = city.PointsOfInterest.FirstOrDefault(pointofInterest => pointofInterest.Id == pointOfInterestId);
            if (pointOfInterstInStore == null)
            {
                return NotFound();
            }

            pointOfInterstInStore.Name = pointOfInterest.Name;
            pointOfInterstInStore.Description = pointOfInterest.Description;
            return NoContent();
        }
    }
}
