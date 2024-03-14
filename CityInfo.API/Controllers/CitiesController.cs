using CityInfo.API.Model;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCities()
        {
            return Ok(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}")]


        public ActionResult<CityDto> GetCity(int id)
        {

            var response = CitiesDataStore.Current.Cities.Find(city => city.Id == id);
            if (response == null)
                return NotFound();

            return Ok(response);


        }
    }
}
