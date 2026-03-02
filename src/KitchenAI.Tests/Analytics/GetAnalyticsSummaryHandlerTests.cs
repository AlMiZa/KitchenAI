using System.Text.Json;
using KitchenAI.Application.Analytics;
using KitchenAI.Domain.Entities;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Tests.Analytics;

public class GetAnalyticsSummaryHandlerTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task WithNoEvents_ReturnsZeroSummary()
    {
        // Arrange
        await using var db = CreateDb();
        var householdId = Guid.NewGuid();
        var handler = new GetAnalyticsSummaryHandler(db);

        // Act
        var result = await handler.Handle(
            new GetAnalyticsSummaryQuery(householdId), CancellationToken.None);

        // Assert
        Assert.Equal(0m, result.MoneySavedEstimate);
        Assert.Equal(0, result.ExpiredItemsCount);
        Assert.Equal(0, result.RecipesGeneratedCount);
        Assert.Empty(result.MostUsedIngredients);
    }

    [Fact]
    public async Task RecipesGeneratedCount_CountsOnlyRecipeGeneratedEvents()
    {
        // Arrange
        await using var db = CreateDb();
        var householdId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        db.AnalyticsEvents.AddRange(
            new AnalyticsEvent { Id = Guid.NewGuid(), HouseholdId = householdId, EventType = "recipe_generated", CreatedAt = now },
            new AnalyticsEvent { Id = Guid.NewGuid(), HouseholdId = householdId, EventType = "recipe_generated", CreatedAt = now },
            new AnalyticsEvent { Id = Guid.NewGuid(), HouseholdId = householdId, EventType = "recipe_generated", CreatedAt = now },
            new AnalyticsEvent { Id = Guid.NewGuid(), HouseholdId = householdId, EventType = "recipe_saved", CreatedAt = now });
        await db.SaveChangesAsync();

        var handler = new GetAnalyticsSummaryHandler(db);

        // Act
        var result = await handler.Handle(
            new GetAnalyticsSummaryQuery(householdId), CancellationToken.None);

        // Assert
        Assert.Equal(3, result.RecipesGeneratedCount);
    }

    [Fact]
    public async Task MostUsedIngredients_Top5FromItemAddedEvents()
    {
        // Arrange
        await using var db = CreateDb();
        var householdId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        // Add Tomato 3 times, Pasta 2 times, Onion once — Tomato and Pasta should appear in top results
        foreach (var name in new[] { "Tomato", "Tomato", "Tomato", "Pasta", "Pasta", "Onion" })
        {
            db.AnalyticsEvents.Add(new AnalyticsEvent
            {
                Id = Guid.NewGuid(),
                HouseholdId = householdId,
                EventType = "item_added",
                Metadata = JsonSerializer.Serialize(new { Name = name }),
                CreatedAt = now
            });
        }
        await db.SaveChangesAsync();

        var handler = new GetAnalyticsSummaryHandler(db);

        // Act
        var result = await handler.Handle(
            new GetAnalyticsSummaryQuery(householdId), CancellationToken.None);

        // Assert
        Assert.Contains("Tomato", result.MostUsedIngredients);
        Assert.Contains("Pasta", result.MostUsedIngredients);
        Assert.Contains("Onion", result.MostUsedIngredients);
        // Tomato (3 uses) must rank above Pasta (2 uses) which must rank above Onion (1 use)
        Assert.True(
            result.MostUsedIngredients.IndexOf("Tomato") < result.MostUsedIngredients.IndexOf("Pasta"),
            "Tomato should rank above Pasta");
        Assert.True(
            result.MostUsedIngredients.IndexOf("Pasta") < result.MostUsedIngredients.IndexOf("Onion"),
            "Pasta should rank above Onion");
    }

    [Fact]
    public async Task MoneySavedEstimate_SumsConsumedItemPrices()
    {
        // Arrange
        await using var db = CreateDb();
        var householdId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        foreach (var price in new[] { 5.0m, 3.5m, 2.0m })
        {
            db.AnalyticsEvents.Add(new AnalyticsEvent
            {
                Id = Guid.NewGuid(),
                HouseholdId = householdId,
                EventType = "item_consumed",
                Metadata = JsonSerializer.Serialize(new { Price = price }),
                CreatedAt = now
            });
        }
        await db.SaveChangesAsync();

        var handler = new GetAnalyticsSummaryHandler(db);

        // Act
        var result = await handler.Handle(
            new GetAnalyticsSummaryQuery(householdId), CancellationToken.None);

        // Assert
        Assert.Equal(10.5m, result.MoneySavedEstimate);
    }
}
