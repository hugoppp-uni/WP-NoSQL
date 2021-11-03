using System.Collections.Generic;
using backend.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StackExchange.Redis;

namespace backend.Services
{
    public class MongoCityService : ICityService
    {
        private readonly IMongoDatabase _mongo;
        private readonly ILogger<MongoCityService> _logger;

        public MongoCityService(IMongoClient mongoClient, ILogger<MongoCityService> logger)
        {
            _mongo = mongoClient.GetDatabase("CityService");
            _logger = logger;

            logger.LogInformation("started city service mongo");
        }


        public City? GetCityFromZip(string zip)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> GetZipsFromCity(string city)
        {
            throw new System.NotImplementedException();
        }
    }
}
