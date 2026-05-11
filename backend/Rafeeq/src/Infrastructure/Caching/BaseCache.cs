using System.Collections.Concurrent;
using Application.Common.Interfaces.Services;
using Application.DTOs.Common;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Caching;

internal class BaseCache : ICacheService
{
    private readonly static ConcurrentDictionary<string, bool> CachedKeys = new();

    protected readonly string Prefix = null!;
    protected readonly TimeSpan ShortTtl2_Minutes = TimeSpan.FromMinutes(2);
    protected readonly TimeSpan ShortTtl5_Minutes = TimeSpan.FromMinutes(5);
    protected readonly TimeSpan MediumTtl10_Minutes = TimeSpan.FromMinutes(10);
    protected readonly TimeSpan MediumTtl20_Minutes = TimeSpan.FromMinutes(20);
    protected readonly TimeSpan LongTtl30_Minutes = TimeSpan.FromMinutes(30);
    protected readonly TimeSpan LongTtl40_Minutes = TimeSpan.FromMinutes(40);
    protected readonly TimeSpan LongTtl2_Hours = TimeSpan.FromHours(2);
    protected readonly TimeSpan LongTtl6_Hours = TimeSpan.FromHours(6);
    private readonly IMemoryCache cache = null!;

    public BaseCache(IMemoryCache cache)
    {
        this.cache = cache;
    }
    
    protected BaseCache(string prefix, IMemoryCache cache)
    {
        this.cache = cache;
        Prefix = prefix;
    }

    protected async Task<T> GetOrCreateAsync<T>(string key, TimeSpan ttl, Func<Task<T>> factory)
        where T : notnull
    {
        var result = await cache.GetOrCreateAsync(key, async entry =>
        {
            CachedKeys.TryAdd(key, true);
            entry.AbsoluteExpirationRelativeToNow = ttl;
            return await factory();
        });

        return result!;
    }

    protected async Task<T?> GetOrCreateNullableAsync<T>(string key, TimeSpan ttl, Func<Task<T?>> factory)
    {
        return await cache.GetOrCreateAsync(key, async entry =>
        {
            CachedKeys.TryAdd(key, true);
            entry.AbsoluteExpirationRelativeToNow = ttl;
            return await factory();
        });
    }

    protected static string FormatPaging(PagingParameters paging)
    {
        return $"p{paging.PageNumber}:s{paging.PageSize}";
    }

    public Task RemoveAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return Task.CompletedTask;

        cache.Remove(key);
        CachedKeys.TryRemove(key, out _);

        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            return Task.CompletedTask;

        var matchingKeys = CachedKeys.Keys
            .Where(k => k.StartsWith(prefix, StringComparison.Ordinal))
            .ToArray();

        foreach (var key in matchingKeys)
        {
            cache.Remove(key);
            CachedKeys.TryRemove(key, out _);
        }

        return Task.CompletedTask;
    }

    public Task RemoveByIdAsync(string prefix, string id)
    {
        if (string.IsNullOrWhiteSpace(prefix) ||
            string.IsNullOrWhiteSpace(id))
        {
            return Task.CompletedTask;
        }

        var expectedSegment = $":{id}";

        var matchingKeys = CachedKeys.Keys
            .Where(k =>
                k.StartsWith(prefix, StringComparison.Ordinal) &&
                k.Contains(expectedSegment, StringComparison.Ordinal))
            .ToArray();

        foreach (var key in matchingKeys)
        {
            cache.Remove(key);
            CachedKeys.TryRemove(key, out _);
        }

        return Task.CompletedTask;
    }
}