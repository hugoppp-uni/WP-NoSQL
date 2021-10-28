using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using StackExchange.Redis;

namespace backend
{
    public class PlzData
    {
        public static void ImportToRedis(IConnectionMultiplexer redis)
        {
            var db = redis.GetDatabase();
            const string path = "res\\plz.data";
            string combine = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, path);
            string[] lines = File.ReadAllLines(combine);
            KeyValuePair<RedisKey, RedisValue>[] keyValuePairs =
                lines
                    .Select(Selector)
                    .ToArray();

            db.StringSet(keyValuePairs);
        }

        private static KeyValuePair<RedisKey, RedisValue> Selector(string json)
        {
            JsonElement dict = JsonSerializer.Deserialize<JsonElement>(json);
            string? key = dict.GetProperty("_id").GetString();
            string? value = dict.GetProperty("city").GetString();

            return new KeyValuePair<RedisKey, RedisValue>(key, value);
        }

    }
}
