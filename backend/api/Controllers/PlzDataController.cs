using System.Collections.Generic;
using System.Linq;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    // [Route("[controller]")]
    public class PlzDataController : ControllerBase
    {
        private readonly CityService _cityService;

        public PlzDataController(CityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet("city/{plz}")]
        public ActionResult<City> Get(string plz)
        {
            City? cityFromPlz = _cityService.GetCityFromPlz(plz);
            if (cityFromPlz is null) return new EmptyResult();

            return cityFromPlz;
        }

        [HttpGet("plz/{city}")]
        public ActionResult<IEnumerable<string>> GetPlz(string city)
        {
            return _cityService.GetPlzsFromCity(city.ToUpper()).ToArray();
        }
    }
}
