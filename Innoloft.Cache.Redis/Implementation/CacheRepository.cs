using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Innoloft.Cache.Redis.Implementation;

public class CacheRepository : ICacheRepository
{
    private readonly IDistributedCache _cache;

    public CacheRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var cachedValue = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(cachedValue))
        {
            return default(T);
        }

        return JsonSerializer.Deserialize<T>(cachedValue);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null)
    {
        var options = new DistributedCacheEntryOptions();

        if (absoluteExpireTime.HasValue)
        {
            options.SetAbsoluteExpiration(absoluteExpireTime.Value);
        }

        if (unusedExpireTime.HasValue)
        {
            options.SetSlidingExpiration(unusedExpireTime.Value);
        }

        var serializedValue = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, serializedValue, options);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}