using Cloud.Core;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cloud.Infrastructure.Tests
{
    public class InMemoryCachingServiceTests
    {
        private const string CacheKey = "order";

        private readonly InMemoryCachingService _cachingService;

        public InMemoryCachingServiceTests()
        {
            _cachingService = new InMemoryCachingService(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Should_cache_data()
        {
            var order = new Order("test", 42m);

            _cachingService.Set(CacheKey, order);
            var result = _cachingService.TryGet<Order>(CacheKey, out var cached);

            Assert.True(result);
            Assert.Same(order, cached);
        }

        [Fact]
        public void Should_delete_key()
        {
            var order = new Order("test", 42m);

            _cachingService.Set(CacheKey, order);
            _cachingService.DeleteKey(CacheKey);
            var result = _cachingService.TryGet<Order>(CacheKey, out var cached);

            Assert.False(result);
            Assert.Null(cached);
        }

        [Fact]
        public async Task Should_invalidate_data()
        {
            var order = new Order("test", 42m);

            _cachingService.Set(CacheKey, order);

            await Task.Delay(TimeSpan.FromSeconds(1));

            var result = _cachingService.TryGet<Order>(CacheKey, out var cached);

            Assert.False(result);
            Assert.Null(cached);
        }
    }
}
