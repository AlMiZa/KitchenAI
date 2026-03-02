using System.Text.Json;
using KitchenAI.Application.Recipes;
using KitchenAI.Domain.Entities;
using KitchenAI.Domain.Enums;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Tests.Recipes;

public class CookRecipeHandlerTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task WithValidRecipe_RecordsAnalyticsEvent()
    {
        // Arrange
        await using var db = CreateDb();
        var householdId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        db.Recipes.Add(new Recipe
        {
            Id = recipeId,
            HouseholdId = householdId,
            Title = "Pasta Primavera",
            Source = RecipeSource.Generated,
            Servings = 2,
            PrepTime = 10,
            CookTime = 20,
            CreatedAt = now,
            UpdatedAt = now
        });
        await db.SaveChangesAsync();

        var handler = new CookRecipeHandler(db);

        // Act
        await handler.Handle(new CookRecipeCommand(householdId, recipeId), CancellationToken.None);

        // Assert
        var evt = await db.AnalyticsEvents.SingleAsync();
        Assert.Equal("recipe_cooked", evt.EventType);
        Assert.Equal(householdId, evt.HouseholdId);
        var meta = JsonSerializer.Deserialize<JsonElement>(evt.Metadata!);
        Assert.Equal("Pasta Primavera", meta.GetProperty("recipeTitle").GetString());
    }

    [Fact]
    public async Task WithNonExistentRecipe_ThrowsKeyNotFoundException()
    {
        // Arrange
        await using var db = CreateDb();
        var handler = new CookRecipeHandler(db);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.Handle(
                new CookRecipeCommand(Guid.NewGuid(), Guid.NewGuid()),
                CancellationToken.None));
    }
}
