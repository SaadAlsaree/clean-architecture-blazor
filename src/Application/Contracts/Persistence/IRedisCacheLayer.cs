using StackExchange.Redis;

namespace Application.Contracts.Persistence;

/// <summary>
/// Enhanced Redis cache layer interface providing comprehensive caching operations
/// with async support, type safety, and advanced Redis features
/// </summary>
public interface IRedisCacheLayer : IDisposable
{
    #region Basic Operations (Async)

    /// <summary>
    /// Asynchronously retrieves a value from cache
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <returns>The cached value or null if not found</returns>
    Task<RedisValue?> GetAsync(string key);

    /// <summary>
    /// Asynchronously retrieves a typed value from cache
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="key">The cache key</param>
    /// <returns>The typed cached value or null if not found</returns>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Asynchronously sets a value in cache with expiration
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="value">The value to cache</param>
    /// <param name="expiration">Optional expiration time</param>
    /// <returns>True if successful</returns>
    Task<bool> SetAsync(string key, string value, TimeSpan? expiration = null);

    /// <summary>
    /// Asynchronously sets a typed value in cache with expiration
    /// </summary>
    /// <typeparam name="T">The type to serialize</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="value">The typed value to cache</param>
    /// <param name="expiration">Optional expiration time</param>
    /// <returns>True if successful</returns>
    Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Asynchronously deletes a key from cache
    /// </summary>
    /// <param name="key">The cache key to delete</param>
    /// <returns>True if the key was deleted</returns>
    Task<bool> DeleteAsync(string key);

    #endregion

    #region Bulk Operations

    /// <summary>
    /// Asynchronously retrieves multiple values from cache
    /// </summary>
    /// <param name="keys">The cache keys to retrieve</param>
    /// <returns>Dictionary of key-value pairs</returns>
    Task<Dictionary<string, RedisValue>> GetMultipleAsync(IEnumerable<string> keys);

    /// <summary>
    /// Asynchronously retrieves multiple typed values from cache
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="keys">The cache keys to retrieve</param>
    /// <returns>Dictionary of key-value pairs</returns>
    Task<Dictionary<string, T?>> GetMultipleAsync<T>(IEnumerable<string> keys) where T : class;

    /// <summary>
    /// Asynchronously sets multiple values in cache
    /// </summary>
    /// <param name="items">Dictionary of key-value pairs to cache</param>
    /// <param name="expiration">Optional expiration time for all items</param>
    /// <returns>Number of successfully cached items</returns>
    Task<int> SetMultipleAsync(Dictionary<string, string> items, TimeSpan? expiration = null);

    /// <summary>
    /// Asynchronously sets multiple typed values in cache
    /// </summary>
    /// <typeparam name="T">The type to serialize</typeparam>
    /// <param name="items">Dictionary of key-value pairs to cache</param>
    /// <param name="expiration">Optional expiration time for all items</param>
    /// <returns>Number of successfully cached items</returns>
    Task<int> SetMultipleAsync<T>(Dictionary<string, T> items, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Asynchronously deletes multiple keys from cache
    /// </summary>
    /// <param name="keys">The cache keys to delete</param>
    /// <returns>Number of successfully deleted keys</returns>
    Task<long> DeleteMultipleAsync(IEnumerable<string> keys);

    #endregion

    #region List Operations

    /// <summary>
    /// Asynchronously retrieves items from a list by key with pagination
    /// </summary>
    /// <param name="key">The list key</param>
    /// <param name="start">Start index (0-based)</param>
    /// <param name="count">Number of items to retrieve</param>
    /// <returns>List of values</returns>
    Task<IEnumerable<RedisValue>> GetListAsync(string key, int start = 0, int count = -1);

    /// <summary>
    /// Asynchronously retrieves typed items from a list by key with pagination
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="key">The list key</param>
    /// <param name="start">Start index (0-based)</param>
    /// <param name="count">Number of items to retrieve</param>
    /// <returns>List of typed values</returns>
    Task<IEnumerable<T>> GetListAsync<T>(string key, int start = 0, int count = -1) where T : class;

    /// <summary>
    /// Asynchronously appends items to a list
    /// </summary>
    /// <param name="key">The list key</param>
    /// <param name="values">Values to append</param>
    /// <returns>New length of the list</returns>
    Task<long> AppendToListAsync(string key, params string[] values);

    /// <summary>
    /// Asynchronously appends typed items to a list
    /// </summary>
    /// <typeparam name="T">The type to serialize</typeparam>
    /// <param name="key">The list key</param>
    /// <param name="values">Typed values to append</param>
    /// <returns>New length of the list</returns>
    Task<long> AppendToListAsync<T>(string key, params T[] values) where T : class;

    #endregion

    #region Key Management

    /// <summary>
    /// Asynchronously checks if a key exists in cache
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <returns>True if the key exists</returns>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// Asynchronously gets the time-to-live for a key
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <returns>TTL in seconds, or null if key doesn't exist or has no expiration</returns>
    Task<TimeSpan?> GetTtlAsync(string key);

    /// <summary>
    /// Asynchronously sets expiration for a key
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="expiration">Expiration time</param>
    /// <returns>True if successful</returns>
    Task<bool> SetExpirationAsync(string key, TimeSpan expiration);

    /// <summary>
    /// Asynchronously removes expiration from a key (makes it persistent)
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <returns>True if successful</returns>
    Task<bool> RemoveExpirationAsync(string key);

    /// <summary>
    /// Asynchronously finds keys matching a pattern
    /// </summary>
    /// <param name="pattern">The search pattern (supports wildcards)</param>
    /// <returns>List of matching keys</returns>
    Task<IEnumerable<string>> FindKeysAsync(string pattern);

    #endregion

    #region Cache Statistics and Monitoring

    /// <summary>
    /// Asynchronously gets cache statistics
    /// </summary>
    /// <returns>Cache statistics information</returns>
    Task<CacheStatistics> GetStatisticsAsync();

    /// <summary>
    /// Asynchronously checks if the Redis connection is healthy
    /// </summary>
    /// <returns>True if connection is healthy</returns>
    Task<bool> IsHealthyAsync();

    /// <summary>
    /// Asynchronously clears all cache data (use with caution)
    /// </summary>
    /// <returns>True if successful</returns>
    Task<bool> ClearAllAsync();

    #endregion

    #region Legacy Synchronous Methods (for backward compatibility)

    /// <summary>
    /// Synchronously retrieves a value from cache (legacy method)
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="value">The output value</param>
    /// <returns>True if the key exists</returns>
    [Obsolete("Use GetAsync instead for better performance")]
    bool TryGet(string key, out RedisValue value);

    /// <summary>
    /// Synchronously retrieves items from a list (legacy method)
    /// </summary>
    /// <param name="key">The list key</param>
    /// <param name="count">Number of items to retrieve</param>
    /// <returns>List of values</returns>
    [Obsolete("Use GetListAsync instead for better performance")]
    IEnumerable<RedisValue> GetPerKey(string key, int count);

    /// <summary>
    /// Synchronously sets an item in cache (legacy method)
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="value">The value to cache</param>
    /// <param name="timeSpan">Expiration time</param>
    /// <returns>True if successful</returns>
    [Obsolete("Use SetAsync instead for better performance")]
    bool SetItem(string key, string value, TimeSpan timeSpan);

    /// <summary>
    /// Synchronously deletes an item from cache (legacy method)
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <returns>True if successful</returns>
    [Obsolete("Use DeleteAsync instead for better performance")]
    bool DeleteItem(string key);

    /// <summary>
    /// Synchronously appends an item to a list (legacy method)
    /// </summary>
    /// <param name="key">The list key</param>
    /// <param name="value">The value to append</param>
    [Obsolete("Use AppendToListAsync instead for better performance")]
    void AppendItemsToKey(string key, string value);

    #endregion
}

/// <summary>
/// Cache statistics information
/// </summary>
public class CacheStatistics
{
    public long TotalKeys { get; set; }
    public long MemoryUsage { get; set; }
    public long HitCount { get; set; }
    public long MissCount { get; set; }
    public double HitRate => TotalRequests > 0 ? (double)HitCount / TotalRequests : 0;
    public long TotalRequests => HitCount + MissCount;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}