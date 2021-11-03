using System.Collections.Generic;
using System.Linq;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("Redis")]
    public class PlzDataController : ControllerBase
    {
        private readonly ICityService _cityService;

        public PlzDataController(ICityService cityService)
        {
            _cityService = cityService;
        }

        /// <summary>
        /// Retrieves the name and state of a city based on the zip code.
        /// </summary>
        /// <param name="zipCode">The zip code of the city to retrieve</param>
        /// <remarks>Returns code 500 when input is not numeric or length is not 5. Returns </remarks>
        [HttpGet("city/{zipCode}")]
        public ActionResult<City> GetCity(string zipCode)
        {
            City? cityFromZip = _cityService.GetCityFromZip(zipCode);
            if (cityFromZip is null) return new EmptyResult();

            return cityFromZip;
        }

        /// <summary>
        /// Retrieves a list of zip codes of a city.
        /// </summary>
        /// <param name="cityName">The name of the city</param>
        [HttpGet("zip/{cityName}")]
        public ActionResult<IEnumerable<string>> GetZips(string cityName)
        {
            return _cityService.GetZipsFromCity(cityName.ToUpper()).ToArray();
        }
    }
}
