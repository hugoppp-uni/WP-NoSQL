using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using MongoDB.Driver;
using StackExchange.Redis;

namespace backend
{
    /// <summary>
    /// Methods for creating a connection to databases.
    /// The database is always flushed first.
    /// </summary>
    public static class ConnectionCreator
    {

        private static bool IsLocalDevEnvironment => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == null;

        public static ConnectionMultiplexer Redis()
        {
            ConnectionMultiplexer redisConnectionMultiplexer;
            try
            {
                string connectionString = IsLocalDevEnvironment ? "localhost,allowAdmin=true" : "redis-db,allowAdmin=true";

                redisConnectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
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
                string connectionString = IsLocalDevEnvironment ? "mongodb://localhost:27017" : "mongodb://mongo-db:27017";

                var client = new MongoClient(connectionString);
                IEnumerable<Task> dropAllDatabaseTasks = client.ListDatabaseNames().ToEnumerable().Select(x => client.DropDatabaseAsync(x));
                Task.WhenAll(dropAllDatabaseTasks);
                return client;
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to mongodb", e);
            }
        }

        public static ISession Cassandra()
        {
            try
            {
                const string keyspace = "CITY";
                string contactPoint = IsLocalDevEnvironment ? "localhost" : "cassandra-db";
                Cluster cluster = Cluster.Builder()
                    .WithDefaultKeyspace(keyspace)
                    .AddContactPoint(contactPoint)
                    .Build();

                // delete and recreate keyspace
                using (ISession cassandra = cluster.Connect(""))
                {
                    cassandra.DeleteKeyspaceIfExists(keyspace);
                    cassandra.CreateKeyspace(keyspace);
                }

                return cluster.Connect();
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to cassandra", e);
            }
        }
    }
}
