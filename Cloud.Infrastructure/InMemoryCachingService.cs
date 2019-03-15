using Cloud.Caching;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Cloud.Infrastructure
{
    public class InMemoryCachingService : ICachingService, IDisposable
    {
        private readonly TimeSpan _absoluteExpirationRelativeToNow;
        private readonly MemoryCache _memoryCache;

        public InMemoryCachingService(TimeSpan expirationScanFrequency, TimeSpan absoluteExpirationRelativeToNow)
        {
            _absoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
            _memoryCache = new MemoryCache(new MemoryCacheOptions
            {
                ExpirationScanFrequency = expirationScanFrequency
            });
        }

        public bool TryGet<T>(string key, out T value)
            where T : class
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        public void Set<T>(string key, T value)
            where T : class
        {
            _memoryCache.Set(key, value, _absoluteExpirationRelativeToNow);
        }

        public void DeleteKey(string key)
        {
            _memoryCache.Remove(key);
        }

        public void Dispose()
        {
            _memoryCache.Dispose();
        }
    }
}
