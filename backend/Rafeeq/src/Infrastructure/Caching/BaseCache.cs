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
    protected readonly TimeSpan LongTtl12_Hours = TimeSpan.FromHours(12);
    private readonly IMemoryCache _cache;

    public BaseCache(IMemoryCache cache)
    {
        _cache = cache;
    }
    
    protected BaseCache(string prefix, IMemoryCache cache)
    {
        _cache = cache;
        Prefix = prefix;
    }

    protected async Task<T> GetOrCreateAsync<T>(string key, TimeSpan ttl, Func<Task<T>> factory)
        where T : notnull
    {
        var result = await _cache.GetOrCreateAsync(key, async entry =>
        {
            CachedKeys.TryAdd(key, true);
            entry.SlidingExpiration = ttl;
            entry.AbsoluteExpirationRelativeToNow = LongTtl12_Hours;
            return await factory();
        });

        return result!;
    }

    protected async Task<T?> GetOrCreateNullableAsync<T>(string key, TimeSpan ttl, Func<Task<T?>> factory)
    {
        return await _cache.GetOrCreateAsync(key, async entry =>
        {
            CachedKeys.TryAdd(key, true);
            entry.SlidingExpiration = ttl;
            entry.AbsoluteExpirationRelativeToNow = LongTtl12_Hours;
            return await factory();
        });
    }

    protected static string FormatPaging(PagingParameters paging)
    {
        return $"p{paging.Page}:s{paging.PageSize}";
    }

    public Task RemoveAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return Task.CompletedTask;

        _cache.Remove(key);
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
            _cache.Remove(key);
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
            _cache.Remove(key);
            CachedKeys.TryRemove(key, out _);
        }

        return Task.CompletedTask;
    }
}