using System.Collections.Generic;
using backend.Models;
using Cassandra;
using Microsoft.Extensions.Logging;

namespace backend.Services
{

    public class CassandraCityService : ICityService
    {
        private readonly ISession _cassandra;

        public CassandraCityService(ICluster cassandra, ILogger<CassandraCityService> logger)
        {
            _cassandra = cassandra.Connect();
            ImportToCassandra(_cassandra);
            logger.LogInformation("Imported data to cassandra");
        }

        public City? GetCityFromZip(string zip)
        {
            return null;
        }

        public IEnumerable<string> GetZipsFromCity(string city)
        {
            yield return _cassandra.Keyspace;
        }

        private static void ImportToCassandra(ISession cassandra)
        {

        }

    }
}
