using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using backend.Content;
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
            
            JsonElement[] jsons = File.ReadAllLines(ContentPath.PlzData)
                .Select(line => JsonSerializer.Deserialize<JsonElement>(line))
                .ToArray();

            List<MongoCity> mongoCities = new();
            foreach (var jsonElement in jsons)
            {
                String zip = jsonElement.GetString("_id");
                String cityName = jsonElement.GetString("city");
                String state = jsonElement.GetString("state");
                mongoCities.Add(new MongoCity(){Name = cityName, State = state, Zip=zip});
            }
            
            _cityCollection.InsertMany(mongoCities);
            _logger.LogInformation("Imported Data to Mongo");
        }

        public City? GetCityFromZip(string zip)
        {
            if (!zip.All(char.IsDigit))
                throw new ArgumentException($"'{zip}' is not numeric", nameof(zip));
            if (zip.Length != 5)
                throw new ArgumentException($"'{zip}' with length '{zip.Length}') is invalid", nameof(zip));

            List<MongoCity> queryResultList = _cityCollection.FindSync(city => city.Zip == zip).ToList();

            if (!queryResultList.Any()) 
            {
                return null; // If no matches were found
            }

            MongoCity queryResult = queryResultList.First();
            return new City() {Name = queryResult.Name, State = queryResult.State};
        }

        public IEnumerable<string> GetZipsFromCity(string cityName)
        {
            List<MongoCity> queryResultList = _cityCollection.FindSync(city => city.Name == cityName).ToList();
            List<String> resultList = new();
            foreach (var mongoCity in queryResultList)
            {
                resultList.Add(mongoCity.Zip);
            }

            return resultList;
        }
    }
}
