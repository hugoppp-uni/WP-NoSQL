using System.Diagnostics;
using System.Linq;
using StackExchange.Redis;

namespace backend
{
    public static class Extensions
    {
        public static IServer GetServer(this IConnectionMultiplexer redisConnectionMultiplexer)
        {
            return redisConnectionMultiplexer.GetServer(redisConnectionMultiplexer.GetEndPoints().First());
        }
    }

}
