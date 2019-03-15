using Cloud.Caching;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace Cloud.Infrastructure
{
    public class RedisCachingService : ICachingService
    {
        private readonly IDatabase _database;
        private readonly TimeSpan _expiry;

        public RedisCachingService(IDatabase database, TimeSpan expiry)
        {
            _database = database;
            _expiry = expiry;
        }

        public bool TryGet<T>(string key, out T value) where T : class
        {
            try
            {
                var redisValue = _database.StringGet(key);
                if (redisValue.HasValue)
                {
                    value = JsonConvert.DeserializeObject<T>(redisValue);
                    return true;
                }

                value = null;
                return false;
            }
            catch (Exception exception)
            {
                value = null;
                return false;
            }
        }

        public void Set<T>(string key, T value) where T : class
        {
            var serialized = JsonConvert.SerializeObject(value);
            _database.StringSet(key, serialized, _expiry);
        }

        public void DeleteKey(string key)
        {
            _database.KeyDelete(key);
        }
    }
}
