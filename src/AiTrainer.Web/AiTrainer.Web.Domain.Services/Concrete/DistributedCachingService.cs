using AiTrainer.Web.Domain.Services.Abstract;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AiTrainer.Web.Domain.Services.Concrete
{
    internal class DistributedCachingService : ICachingService
    {
        private static readonly Type _typeofString = typeof(string);
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<DistributedCachingService> _logger;

        public DistributedCachingService(
            IDistributedCache distributedCache,
            ILogger<DistributedCachingService> logger
        )
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public async Task<bool> TryRemoveObject(string key)
        {
            try
            {
                await _distributedCache.RemoveAsync(key);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred removing object from cache");

                return false;
            }
        }

        public async Task<T?> TryGetObject<T>(string key)
            where T : class
        {
            try
            {
                return await GetObject<T>(key);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred getting object from cache");

                return null;
            }
        }

        public async Task<string> SetObject<T>(
            string key,
            T value,
            CacheObjectTimeToLiveInSeconds timeToLive = CacheObjectTimeToLiveInSeconds.ThirtyMinutes
        )
            where T : class
        {
            return await SetObject(
                key,
                value,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds((int)timeToLive),
                }
            );
        }

        public async Task<string> SetObject<T>(
            string key,
            T value,
            DistributedCacheEntryOptions options
        )
            where T : class
        {
            if (typeof(T) == _typeofString)
            {
                await _distributedCache.SetStringAsync(
                    key,
                    (value as string)!,
                    options ?? new DistributedCacheEntryOptions()
                );
            }
            else
            {
                var serializedValue = JsonSerializer.Serialize(value);
                await _distributedCache.SetStringAsync(
                    key,
                    serializedValue,
                    options ?? new DistributedCacheEntryOptions()
                );
            }
            return key;
        }

        private async Task<T> GetObject<T>(string key)
            where T : class
        {
            var foundValue =
                await _distributedCache.GetStringAsync(key)
                ?? throw new InvalidOperationException("Cannot find object with that key");
            if (typeof(T) == _typeofString)
            {
                return foundValue as T ?? throw new InvalidDataException("Cannot parse object");
            }
            return JsonSerializer.Deserialize<T>(foundValue)
                ?? throw new InvalidDataException("Cannot parse object");
        }
    }

    public enum CacheObjectTimeToLiveInSeconds
    {
        OneMinute = 60,
        FiveMinutes = 300,
        TenMinutes = 600,
        ThirtyMinutes = 1800,
        OneHour = 3600,
    }
}
