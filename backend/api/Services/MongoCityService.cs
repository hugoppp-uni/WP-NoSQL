using System;
using System.Collections.Generic;
using System.Linq;
using backend.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace backend.Services
{
    public class MongoCityService : ICityService
    {
        private readonly ILogger<MongoCityService> _logger;
        private IMongoCollection<MongoCity> _cityCollection;

        public MongoCityService(IMongoClient mongoClient, ILogger<MongoCityService> logger)
        {
            _logger = logger;

            var mongoDb = mongoClient.GetDatabase("CityService");
            _cityCollection = mongoDb.GetCollection<MongoCity>("citycollection");

            _cityCollection.InsertOne(new MongoCity(){Name = "test", State = "NA"});
            _cityCollection.InsertOne(new MongoCity(){Name = "asdf", State = "LA"});

            logger.LogInformation("started city service mongo");
        }


        public City? GetCityFromZip(string zip)
        {
            return _cityCollection.FindSync(city => city.State == "NA").First();
        }

        public IEnumerable<string> GetZipsFromCity(string city)
        {
            throw new NotImplementedException();
        }
    }
}
