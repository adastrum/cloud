using Cloud.Caching;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;

namespace Cloud.Infrastructure
{
    public class InMemoryCachingService : ICachingService, IDisposable
    {
        private readonly MemoryCache _memoryCache;

        public InMemoryCachingService()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024,
                ExpirationScanFrequency = TimeSpan.FromMinutes(5)
            });
        }

        public bool TryGet<T>(string key, out T value)
            where T : class
        {
            var success = TryGetString(key, out var valueAsString);

            value = success ? JsonConvert.DeserializeObject<T>(valueAsString) : null;

            return success;
        }

        public void Set<T>(string key, T value)
            where T : class
        {
            _memoryCache.Set(key, value);
        }

        public void DeleteKey(string key)
        {
            _memoryCache.Remove(key);
        }

        private bool TryGetString(string key, out string value)
        {
            var success = _memoryCache.TryGetValue(key, out var result);

            value = result?.ToString();

            return success;
        }

        public void Dispose()
        {
            _memoryCache.Dispose();
        }
    }
}
