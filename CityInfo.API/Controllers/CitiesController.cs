﻿using CityInfo.API.Model;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase

    {
        private readonly CitiesDataStore _citiesDataStore;

        public CitiesController(CitiesDataStore citiesDataStore)
        {
            _citiesDataStore = citiesDataStore;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCities()
        {
            return Ok(_citiesDataStore.Cities);
        }

        [HttpGet("{id}")]


        public ActionResult<CityDto> GetCity(int id)
        {

            var response = _citiesDataStore.Cities.Find(city => city.Id == id);
            if (response == null)
                return NotFound();

            return Ok(response);


        }
    }
}
