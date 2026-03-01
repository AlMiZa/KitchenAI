using System.Text.Json;
using KitchenAI.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Analytics;

/// <summary>Computes the analytics summary for a household from stored events and items.</summary>
public class GetAnalyticsSummaryHandler(IAppDbContext db)
    : IRequestHandler<GetAnalyticsSummaryQuery, AnalyticsSummaryDto>
{
    private sealed record ItemAddedMeta(string? Name);
    private sealed record ItemConsumedMeta(decimal? Price);

    /// <inheritdoc/>
    public async Task<AnalyticsSummaryDto> Handle(
        GetAnalyticsSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var expiredCount = await db.Items
            .CountAsync(i => i.HouseholdId == request.HouseholdId
                             && i.ExpiryDate.HasValue
                             && i.ExpiryDate.Value < today, cancellationToken);

        var events = await db.AnalyticsEvents
            .Where(e => e.HouseholdId == request.HouseholdId)
            .ToListAsync(cancellationToken);

        var recipesGeneratedCount = events.Count(e => e.EventType == "recipe_generated");

        // Aggregate most-used ingredients from item_added events
        var ingredientCounts = events
            .Where(e => e.EventType == "item_added" && e.Metadata is not null)
            .Select(e =>
            {
                try { return JsonSerializer.Deserialize<ItemAddedMeta>(e.Metadata!)?.Name; }
                catch { return null; }
            })
            .Where(name => name is not null)
            .GroupBy(name => name!)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => g.Key)
            .ToList();

        // Estimate money saved = sum of prices of consumed items
        var moneySaved = events
            .Where(e => e.EventType == "item_consumed" && e.Metadata is not null)
            .Sum(e =>
            {
                try { return JsonSerializer.Deserialize<ItemConsumedMeta>(e.Metadata!)?.Price ?? 0m; }
                catch { return 0m; }
            });

        return new AnalyticsSummaryDto(moneySaved, expiredCount, recipesGeneratedCount, ingredientCounts);
    }
}
