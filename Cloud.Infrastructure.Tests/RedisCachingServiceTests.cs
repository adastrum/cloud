using StackExchange.Redis;
using System;

namespace Cloud.Infrastructure.Tests
{
    public class RedisCachingServiceTests : CachingServiceTestsBase
    {
        public RedisCachingServiceTests()
        {
            var redis = ConnectionMultiplexer.Connect("localhost");
            var database = redis.GetDatabase();
            CachingService = new RedisCachingService(
                database,
                TimeSpan.FromSeconds(ExpirationInSeconds));
        }
    }
}
