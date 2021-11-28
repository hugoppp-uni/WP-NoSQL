using System;
using System.Collections.Generic;
using System.Linq;
using backend.Models;
using Microsoft.AspNetCore.Authentication;
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

            ImportToMongo();
        }

        private void ImportToMongo()
        {
            IEnumerable<MongoCity> mongoCities = Content.PlzData
                .Select(jsonElement => new MongoCity()
                {
                    Name = jsonElement.GetString("city") ?? "",
                    State = jsonElement.GetString("state") ?? "",
                    Zip = jsonElement.GetString("_id") ?? throw new ArgumentException("no zip provided")
                });

            _cityCollection.InsertMany(mongoCities);
            _logger.LogInformation("Imported Data to Mongo");
        }

        public City? GetCityFromZip(string zip)
        {
            ICityService.ValidateZip(zip);

            MongoCity? queryResultCity = _cityCollection
                .FindSync(city => city.Zip == zip)
                .FirstOrDefault();

            if (queryResultCity is null) return null;

            return new City() { Name = queryResultCity.Name, State = queryResultCity.State };
        }

        public IEnumerable<string> GetZipsFromCity(string cityName)
        {
            List<MongoCity> queryResultCities = _cityCollection.FindSync(city => city.Name == cityName).ToList();
            return queryResultCities.Select(city => city.Zip);
        }
    }
}
