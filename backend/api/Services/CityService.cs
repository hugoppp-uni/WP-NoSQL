using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using backend.Models;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace backend.Services
{
    public class CityService
    {
        private const string StateKeyPostfix = ".state";
        private const string NameKeyPostfix = ".name";
        private const string PlzKeyPostfix = ".plz";

        private readonly IConnectionMultiplexer _redis;

        public CityService(IConnectionMultiplexer redis, ILogger<CityService> logger)
        {
            _redis = redis;
            ImportToRedis(redis);
            logger.LogInformation("Imported data to Redis");
        }

        public City? GetCityFromPlz(string plz)
        {
            if (!plz.All(char.IsDigit))
                throw new ArgumentException($"'{plz}' is not numeric", nameof(plz));
            if (plz.Length != 5)
                throw new ArgumentException($"'{plz}' with length '{plz.Length}') is invalid", nameof(plz));

            var redisDb = _redis.GetDatabase();
            RedisValue name = redisDb.StringGet(plz + NameKeyPostfix);
            RedisValue state = redisDb.StringGet(plz + StateKeyPostfix);
            if (!name.HasValue && !state.HasValue) return null;

            return new City() { Name = name.ToString(), State = state.ToString() };
        }

        public IEnumerable<string> GetPlzsFromCity(string city)
        {
            var redisDb = _redis.GetDatabase();
            return redisDb.ListRange(city + PlzKeyPostfix).Select(x => x.ToString());
        }

        private static void ImportToRedis(IConnectionMultiplexer redis)
        {
            var db = redis.GetDatabase();
            const string path = "res\\plz.data";
            string combine = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, path);

            JsonElement[] jsons = File.ReadAllLines(combine)
                .Select(line => JsonSerializer.Deserialize<JsonElement>(line))
                .ToArray();

            Task task1 = db.StringSetAsync(jsons.Select(PlzCityNameSelector).ToArray());
            Task task2 = db.StringSetAsync(jsons.Select(PlzStateSelector).ToArray());

            IEnumerable<Task> tasks =
                jsons.Select(CityNamePlzSelector)
                    .GroupBy(x => x.Key) //group by plz
                    .Select(cityNamePlzsPair =>
                        db.ListLeftPushAsync(
                            cityNamePlzsPair.Key, //plz
                            cityNamePlzsPair.Select(x => x.Value).ToArray() //city names
                        ));

            Task.WhenAll(tasks.Prepend(task1).Prepend(task2)).Wait();
        }

        private static KeyValuePair<RedisKey, RedisValue> PlzCityNameSelector(JsonElement json)
        {
            string key = json.GetProperty("_id").GetString() + NameKeyPostfix;
            string? value = json.GetProperty("city").GetString();

            return new KeyValuePair<RedisKey, RedisValue>(key, value);
        }

        private static KeyValuePair<RedisKey, RedisValue> PlzStateSelector(JsonElement json)
        {
            string key = json.GetProperty("_id").GetString() + StateKeyPostfix;
            string? value = json.GetProperty("state").GetString();

            return new KeyValuePair<RedisKey, RedisValue>(key, value);
        }

        private static KeyValuePair<RedisKey, RedisValue> CityNamePlzSelector(JsonElement json)
        {
            string key = json.GetProperty("city").GetString() + PlzKeyPostfix;
            string value = json.GetProperty("_id").GetString();

            return new KeyValuePair<RedisKey, RedisValue>(key, value);
        }
    }
}
