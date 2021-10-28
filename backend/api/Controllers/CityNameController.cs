using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CityNameController : ControllerBase
    {
        private readonly ILogger<CityNameController> _logger;
        private readonly IConnectionMultiplexer _redis;

        public CityNameController(ILogger<CityNameController> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
        }

        [HttpGet("{plz}")]
        public ActionResult Get(string plz)
        {
            if (plz.Length != 5)
                throw new ArgumentException($"Plz length ({plz.Length}) is invalid", nameof(plz));

            var redisDb = _redis.GetDatabase();
            RedisValue redisValue = redisDb.StringGet(plz);
            if (redisValue.IsNullOrEmpty) return new EmptyResult();

            return Content(redisValue.ToString());
        }
    }
}
