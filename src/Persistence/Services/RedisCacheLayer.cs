using StackExchange.Redis;

namespace Persistence.Services;

internal class RedisCacheLayer : IRedisCacheLayer
{
    public void AppendItemsToKey(string key, string value)
    {
        throw new NotImplementedException();
    }

    public Task<long> AppendToListAsync(string key, params string[] values)
    {
        throw new NotImplementedException();
    }

    public Task<long> AppendToListAsync<T>(string key, params T[] values) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<bool> ClearAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(string key)
    {
        throw new NotImplementedException();
    }

    public bool DeleteItem(string key)
    {
        throw new NotImplementedException();
    }

    public Task<long> DeleteMultipleAsync(IEnumerable<string> keys)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> FindKeysAsync(string pattern)
    {
        throw new NotImplementedException();
    }

    public Task<RedisValue?> GetAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetAsync<T>(string key) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RedisValue>> GetListAsync(string key, int start = 0, int count = -1)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> GetListAsync<T>(string key, int start = 0, int count = -1) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, RedisValue>> GetMultipleAsync(IEnumerable<string> keys)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, T?>> GetMultipleAsync<T>(IEnumerable<string> keys) where T : class
    {
        throw new NotImplementedException();
    }

    public IEnumerable<RedisValue> GetPerKey(string key, int count)
    {
        throw new NotImplementedException();
    }

    public Task<CacheStatistics> GetStatisticsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<TimeSpan?> GetTtlAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsHealthyAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveExpirationAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SetAsync(string key, string value, TimeSpan? expiration = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<bool> SetExpirationAsync(string key, TimeSpan expiration)
    {
        throw new NotImplementedException();
    }

    public bool SetItem(string key, string value, TimeSpan timeSpan)
    {
        throw new NotImplementedException();
    }

    public Task<int> SetMultipleAsync(Dictionary<string, string> items, TimeSpan? expiration = null)
    {
        throw new NotImplementedException();
    }

    public Task<int> SetMultipleAsync<T>(Dictionary<string, T> items, TimeSpan? expiration = null) where T : class
    {
        throw new NotImplementedException();
    }

    public bool TryGet(string key, out RedisValue value)
    {
        throw new NotImplementedException();
    }
}
