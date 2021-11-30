using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using Cassandra;
using Cassandra.Mapping;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace backend.Services
{

    public class CassandraCityService : ICityService
    {
        private readonly ISession _cassandra;
        Mapper mapper;

        public CassandraCityService(ISession cassandra, ILogger<CassandraCityService> logger)
        {
            _cassandra = cassandra;
            mapper = new(_cassandra);
            int rowCount = ImportToCassandra(_cassandra);
            logger.LogInformation("Imported {} rows to cassandra", rowCount);
        }


        public City? GetCityFromZip(string zip)
        {
            ICityService.ValidateZip(zip);
            return mapper.SingleOrDefault<City>($"SELECT * from CITY where zip=?", zip);
        }

        public IEnumerable<string> GetZipsFromCity(string city)
        {
            RowSet cityRowSet = _cassandra.Execute(
                _cassandra.Prepare(
                    $"SELECT zip from CITY where name=? ALLOW FILTERING"
                ).Bind(city));

            return cityRowSet.Select(cityRow => cityRow.GetValue<string>("zip"));
        }

        private static int ImportToCassandra(ISession cassandra)
        {
            cassandra.Execute(
                "CREATE TABLE CITY (zip text, name text, state text, soccer text, PRIMARY KEY(zip))"
            );
            // Aufgabe b.): Neue Spalte hinzufügen
            cassandra.Execute("ALTER TABLE CITY ADD Fussball text");
            // Aufgabe b.) Werte für Fussball für die Städte Bremen und Hamburg ändern
            cassandra.Execute("UPDATE CITY SET Fussball = 'Ja' WHERE name = 'BREMEN'");
            cassandra.Execute("UPDATE CITY SET Fussball = 'Ja' WHERE name = 'HAMBURG'");

            IEnumerable<Task<RowSet>> insertTasks =
                Content.PlzData
                    .Select(json => $"INSERT INTO CITY (zip, name, state) " +
                                    $"VALUES ('{json.GetString("_id")}', '{json.GetString("city")}', '{json.GetString("state")}')")
                    .Select(query => cassandra.ExecuteAsync(new SimpleStatement(query)));

            return Task.WhenAll(insertTasks).Result.Length;
        }


    }
}
