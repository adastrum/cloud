using Cloud.Caching;
using Cloud.Core;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cloud.Infrastructure.Tests
{
    public abstract class CachingServiceTestsBase
    {
        private const string CacheKey = "order";
        protected const int ExpirationInSeconds = 1;

        protected ICachingService CachingService;

        [Fact]
        public void Should_cache_data()
        {
            var order = new Order("test", 42m);

            CachingService.Set(CacheKey, order);
            var result = CachingService.TryGet<Order>(CacheKey, out var cached);

            Assert.True(result);
            cached.Should().BeEquivalentTo(order);
        }

        [Fact]
        public void Should_delete_key()
        {
            var order = new Order("test", 42m);

            CachingService.Set(CacheKey, order);
            CachingService.DeleteKey(CacheKey);
            var result = CachingService.TryGet<Order>(CacheKey, out var cached);

            Assert.False(result);
            Assert.Null(cached);
        }

        [Fact]
        public async Task Should_invalidate_data()
        {
            var order = new Order("test", 42m);

            CachingService.Set(CacheKey, order);

            await Task.Delay(TimeSpan.FromSeconds(ExpirationInSeconds + 1));

            var result = CachingService.TryGet<Order>(CacheKey, out var cached);

            Assert.False(result);
            Assert.Null(cached);
        }
    }
}