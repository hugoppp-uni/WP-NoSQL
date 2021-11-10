using System.Collections;
using System.Collections.Generic;
using System.Linq;
using backend;
using backend.Models;
using backend.Services;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Logging.Abstractions;

namespace Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Benchmark>();
        }
    }

    // [ShortRunJob]
    public class Benchmark
    {
        private static RedisCityService _redisCityService = null!;
        private static MongoCityService _mongoCityService = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var redis = ConnectionCreator.Redis();
            _redisCityService = new(redis, NullLogger<RedisCityService>.Instance);

            var mongo = ConnectionCreator.Mongo();
            _mongoCityService = new(mongo, NullLogger<MongoCityService>.Instance);
        }

        [Benchmark]
        public object BenchRedis() => Bench(_redisCityService);

        [Benchmark]
        public object BenchMongo() => Bench(_mongoCityService);

        private static object Bench(ICityService cityService)
        {
            IEnumerable<string> cities = new[] { "HAMBURG", "EAST LIVERMORE", "PINEHURST", "JEFFERSON" };
            IEnumerable<string> zips = new[] { "55339" , "76384", "83455", "93644"};

            string[] resultZips = cities.SelectMany(city => cityService.GetZipsFromCity(city)).ToArray();
            City?[] resultCities = zips.Select(zip => cityService.GetCityFromZip(zip)).ToArray();

            return (resultZips, resultCities);
        }
    }
}
