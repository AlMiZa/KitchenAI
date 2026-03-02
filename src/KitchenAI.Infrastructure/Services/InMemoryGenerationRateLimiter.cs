using KitchenAI.Application.Recipes;
using KitchenAI.Application.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace KitchenAI.Infrastructure.Services;

/// <summary>
/// In-memory sliding-window rate limiter for recipe generation requests.
/// Uses IMemoryCache to track per-household request counts with a per-key lock
/// to prevent concurrent requests from bypassing the limit.
/// </summary>
public class InMemoryGenerationRateLimiter(IMemoryCache cache, IOptions<GenerationRateLimitOptions> options)
    : IGenerationRateLimiter
{
    private readonly int _maxRequestsPerHour = options.Value.MaxRequestsPerHour;

    // Per-household lock objects to prevent race conditions on the read-modify-write cycle.
    private readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, object> _locks = new();

    /// <inheritdoc/>
    public bool TryConsume(Guid householdId)
    {
        var lockObj = _locks.GetOrAdd(householdId, _ => new object());
        lock (lockObj)
        {
            var key = $"gen_rate:{householdId}";
            var count = cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return 0;
            });

            if (count >= _maxRequestsPerHour)
                return false;

            cache.Set(key, count + 1, TimeSpan.FromHours(1));
            return true;
        }
    }
}
