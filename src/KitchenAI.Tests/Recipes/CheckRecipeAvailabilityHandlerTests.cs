using KitchenAI.Application.Recipes;
using KitchenAI.Domain.Entities;
using KitchenAI.Domain.Enums;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Tests.Recipes;

public class CheckRecipeAvailabilityHandlerTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static async Task<(AppDbContext db, Guid householdId, Guid recipeId)> SetupAsync(
        string ingredientName, decimal requiredQty, decimal availableQty)
    {
        var db = CreateDb();
        var householdId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        db.Items.Add(new Item
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            Name = ingredientName,
            Quantity = availableQty,
            Unit = "g",
            CreatedAt = now,
            UpdatedAt = now
        });

        var recipe = new Recipe
        {
            Id = recipeId,
            HouseholdId = householdId,
            Title = "Test Recipe",
            Source = RecipeSource.Generated,
            Servings = 2,
            PrepTime = 5,
            CookTime = 10,
            CreatedAt = now,
            UpdatedAt = now
        };
        db.Recipes.Add(recipe);
        db.RecipeIngredients.Add(new RecipeIngredient
        {
            Id = Guid.NewGuid(),
            RecipeId = recipeId,
            Name = ingredientName,
            Quantity = requiredQty,
            Unit = "g"
        });

        await db.SaveChangesAsync();
        return (db, householdId, recipeId);
    }

    [Fact]
    public async Task AllIngredientsAvailable_ReturnsReady()
    {
        // Arrange
        var (db, householdId, recipeId) = await SetupAsync("Flour", 200, 500);
        await using var _ = db;
        var handler = new CheckRecipeAvailabilityHandler(db);

        // Act
        var result = await handler.Handle(
            new CheckRecipeAvailabilityQuery(householdId, recipeId), CancellationToken.None);

        // Assert
        Assert.Equal("ready", result.Status);
        Assert.NotNull(result.Items);
        Assert.All(result.Items, i => Assert.Equal(0, i.Deficit));
    }

    [Fact]
    public async Task MissingIngredient_ReturnsMissingWithDeficit()
    {
        // Arrange
        var (db, householdId, recipeId) = await SetupAsync("Butter", 300, 100);
        await using var _ = db;
        var handler = new CheckRecipeAvailabilityHandler(db);

        // Act
        var result = await handler.Handle(
            new CheckRecipeAvailabilityQuery(householdId, recipeId), CancellationToken.None);

        // Assert
        Assert.Equal("missing", result.Status);
        Assert.NotNull(result.Items);
        var butter = Assert.Single(result.Items, i => i.Name == "Butter");
        Assert.Equal(200, butter.Deficit);
    }

    [Fact]
    public async Task IngredientEntirelyAbsent_ReturnsMissingWithFullDeficit()
    {
        // Arrange: recipe requires Sugar but inventory has no Sugar at all
        var db = CreateDb();
        var householdId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var recipe = new Recipe
        {
            Id = recipeId,
            HouseholdId = householdId,
            Title = "Test Recipe",
            Source = RecipeSource.Generated,
            Servings = 2,
            PrepTime = 5,
            CookTime = 10,
            CreatedAt = now,
            UpdatedAt = now
        };
        db.Recipes.Add(recipe);
        db.RecipeIngredients.Add(new RecipeIngredient
        {
            Id = Guid.NewGuid(),
            RecipeId = recipeId,
            Name = "Sugar",
            Quantity = 150,
            Unit = "g"
        });
        await db.SaveChangesAsync();

        await using var _ = db;
        var handler = new CheckRecipeAvailabilityHandler(db);

        // Act
        var result = await handler.Handle(
            new CheckRecipeAvailabilityQuery(householdId, recipeId), CancellationToken.None);

        // Assert
        Assert.Equal("missing", result.Status);
        Assert.NotNull(result.Items);
        var sugar = Assert.Single(result.Items, i => i.Name == "Sugar");
        Assert.Equal(0, sugar.Available);
        Assert.Equal(150, sugar.Deficit);
    }
}
