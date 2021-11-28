using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using backend.Models;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace backend.Services
{

    public class RedisCityService : ICityService
    {
        private const string StateKeyPostfix = ".state";
        private const string NameKeyPostfix = ".name";
        private const string ZipKeyPostfix = ".zip";

        private readonly IConnectionMultiplexer _redis;

        public RedisCityService(IConnectionMultiplexer redis, ILogger<RedisCityService> logger)
        {
            _redis = redis;
            ImportToRedis(redis);
            logger.LogInformation("Imported data to Redis");
        }

        public City? GetCityFromZip(string zip)
        {
            ICityService.ValidateZip(zip);

            var redisDb = _redis.GetDatabase();
            RedisValue name = redisDb.StringGet(zip + NameKeyPostfix);
            RedisValue state = redisDb.StringGet(zip + StateKeyPostfix);
            if (!name.HasValue && !state.HasValue) return null;

            return new City() { Name = name.ToString(), State = state.ToString() };
        }

        public IEnumerable<string> GetZipsFromCity(string city)
        {
            var redisDb = _redis.GetDatabase();
            return redisDb.ListRange(city + ZipKeyPostfix).Select(x => x.ToString());
        }

        private static void ImportToRedis(IConnectionMultiplexer redis)
        {
            var db = redis.GetDatabase();

            Task task1 = db.StringSetAsync(Content.PlzData.Select(ZipCityNameSelector).ToArray());
            Task task2 = db.StringSetAsync(Content.PlzData.Select(ZipStateSelector).ToArray());

            IEnumerable<Task> tasks =
                Content.PlzData.Select(CityNameZipSelector)
                    .GroupBy(x => x.Key) //group by zip
                    .Select(cityNameZipsPair =>
                        db.ListLeftPushAsync(
                            cityNameZipsPair.Key, //zip
                            cityNameZipsPair.Select(x => x.Value).ToArray() //city names
                        ));

            Task.WhenAll(tasks.Prepend(task1).Prepend(task2)).Wait();
        }

        private static KeyValuePair<RedisKey, RedisValue> ZipCityNameSelector(JsonElement json)
        {
            string key = json.GetProperty("_id").GetString() + NameKeyPostfix;
            string? value = json.GetProperty("city").GetString();

            return new KeyValuePair<RedisKey, RedisValue>(key, value);
        }

        private static KeyValuePair<RedisKey, RedisValue> ZipStateSelector(JsonElement json)
        {
            string key = json.GetProperty("_id").GetString() + StateKeyPostfix;
            string? value = json.GetProperty("state").GetString();

            return new KeyValuePair<RedisKey, RedisValue>(key, value);
        }

        private static KeyValuePair<RedisKey, RedisValue> CityNameZipSelector(JsonElement json)
        {
            string key = json.GetProperty("city").GetString() + ZipKeyPostfix;
            string value = json.GetProperty("_id").GetString();

            return new KeyValuePair<RedisKey, RedisValue>(key, value);
        }
    }
}
