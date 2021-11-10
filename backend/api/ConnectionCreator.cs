using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using StackExchange.Redis;

namespace backend
{
    public class ConnectionCreator
    {
        public static ConnectionMultiplexer Redis()
        {
            ConnectionMultiplexer redisConnectionMultiplexer;
            try
            {
                redisConnectionMultiplexer = ConnectionMultiplexer.Connect("localhost,allowAdmin=true");
                redisConnectionMultiplexer.GetServer().FlushDatabase();
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to redis", e);
            }

            return redisConnectionMultiplexer;
        }

        public static IMongoClient Mongo()
        {
            try
            {
                var client = new MongoClient("mongodb://localhost:27017");
                IEnumerable<Task> dropAllDatabaseTasks = client.ListDatabaseNames().ToEnumerable().Select(x => client.DropDatabaseAsync(x));
                Task.WhenAll(dropAllDatabaseTasks);
                return client;
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to mongodb", e);
            }
        }
    }
}
