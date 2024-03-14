using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController:ControllerBase
    {
        [HttpGet]
        public JsonResult GetCities()
        {
            return new JsonResult(
                new object[]
                {
                    new {id=1, name="Chennai"}
                    , new {id=2, name="Madurai"}

                }

                );
        }
    }
}
