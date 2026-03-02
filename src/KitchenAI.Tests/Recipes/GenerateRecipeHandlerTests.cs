using KitchenAI.Application.Exceptions;
using KitchenAI.Application.Recipes;
using KitchenAI.Application.Services;
using KitchenAI.Domain.Entities;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace KitchenAI.Tests.Recipes;

public class GenerateRecipeHandlerTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static IMemoryCache CreateCache() =>
        new MemoryCache(Options.Create(new MemoryCacheOptions()));

    private static (Guid householdId, Guid userId) SeedDbWithItems(AppDbContext db)
    {
        var householdId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        db.Users.Add(new User
        {
            Id = userId,
            Email = "test@example.com",
            PasswordHash = "hash",
            DisplayName = "Test User",
            CreatedAt = now,
            UpdatedAt = now
        });
        db.Items.AddRange(
            new Item
            {
                Id = Guid.NewGuid(),
                HouseholdId = householdId,
                Name = "Tomato",
                Quantity = 3,
                Unit = "pcs",
                CreatedAt = now,
                UpdatedAt = now
            },
            new Item
            {
                Id = Guid.NewGuid(),
                HouseholdId = householdId,
                Name = "Pasta",
                Quantity = 500,
                Unit = "g",
                CreatedAt = now,
                UpdatedAt = now
            });
        db.SaveChanges();
        return (householdId, userId);
    }

    private static GenerateRecipeHandler CreateHandler(
        AppDbContext db,
        ILlmService llmService,
        IRecipeAdapter recipeAdapter,
        IGenerationRateLimiter rateLimiter,
        IMemoryCache? cache = null) =>
        new(db, llmService, recipeAdapter, rateLimiter, cache ?? CreateCache());

    [Fact]
    public async Task WithValidItems_ReturnsTwoRecipes()
    {
        // Arrange
        await using var db = CreateDb();
        var (householdId, userId) = SeedDbWithItems(db);

        var llmMock = new Mock<ILlmService>();
        llmMock.Setup(s => s.GenerateRecipesAsync(
                It.IsAny<IList<Item>>(),
                It.IsAny<IList<GeneratedRecipeDto>>(),
                It.IsAny<RecipeConstraints>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new GeneratedRecipeDto(Guid.NewGuid(), "Tomato Pasta",
                    [new RecipeIngredientDto(Guid.NewGuid(), "Tomato", 3, "pcs")],
                    ["Boil pasta.", "Add tomato sauce."], null, null, 10, 20, 4),
                new GeneratedRecipeDto(Guid.NewGuid(), "Pasta Salad",
                    [new RecipeIngredientDto(Guid.NewGuid(), "Pasta", 200, "g")],
                    ["Cook pasta.", "Cool and mix."], null, null, 5, 15, 2)
            ]);

        var adapterMock = new Mock<IRecipeAdapter>();
        adapterMock.Setup(a => a.SearchAsync(
                It.IsAny<IList<string>>(),
                It.IsAny<RecipeConstraints>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var rateLimiterMock = new Mock<IGenerationRateLimiter>();
        rateLimiterMock.Setup(r => r.TryConsume(It.IsAny<Guid>())).Returns(true);

        var handler = CreateHandler(db, llmMock.Object, adapterMock.Object, rateLimiterMock.Object);
        var command = new GenerateRecipeCommand(householdId, userId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Count >= 2);
        Assert.Contains(result, r => r.Title == "Tomato Pasta");
        Assert.Contains(result, r => r.Title == "Pasta Salad");
        Assert.Equal(1, await db.GeneratedRecipes.CountAsync());
    }

    [Fact]
    public async Task WithAdapterMockAndLlmMock_ReturnsTwoRecipes()
    {
        // Arrange
        await using var db = CreateDb();
        var (householdId, userId) = SeedDbWithItems(db);

        var adapterRecipes = new List<GeneratedRecipeDto>
        {
            new(Guid.NewGuid(), "Adapter Recipe 1",
                [new RecipeIngredientDto(Guid.NewGuid(), "Tomato", 3, "pcs")],
                ["Step 1.", "Step 2."], null, null, 5, 10, 4),
            new(Guid.NewGuid(), "Adapter Recipe 2",
                [new RecipeIngredientDto(Guid.NewGuid(), "Pasta", 200, "g")],
                ["Step A.", "Step B."], null, null, 5, 15, 4)
        };

        var adapterMock = new Mock<IRecipeAdapter>();
        adapterMock.Setup(a => a.SearchAsync(
                It.IsAny<IList<string>>(),
                It.IsAny<RecipeConstraints>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(adapterRecipes);

        var llmRecipes = new List<GeneratedRecipeDto>
        {
            new(Guid.NewGuid(), "LLM Recipe 1",
                [new RecipeIngredientDto(Guid.NewGuid(), "Tomato", 2, "pcs")],
                ["Cook it.", "Serve."], null, null, 10, 20, 4),
            new(Guid.NewGuid(), "LLM Recipe 2",
                [new RecipeIngredientDto(Guid.NewGuid(), "Pasta", 100, "g")],
                ["Boil.", "Drain."], null, null, 5, 12, 2)
        };

        var llmMock = new Mock<ILlmService>();
        llmMock.Setup(s => s.GenerateRecipesAsync(
                It.IsAny<IList<Item>>(),
                It.IsAny<IList<GeneratedRecipeDto>>(),
                It.IsAny<RecipeConstraints>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(llmRecipes);

        var rateLimiterMock = new Mock<IGenerationRateLimiter>();
        rateLimiterMock.Setup(r => r.TryConsume(It.IsAny<Guid>())).Returns(true);

        var handler = CreateHandler(db, llmMock.Object, adapterMock.Object, rateLimiterMock.Object);
        var command = new GenerateRecipeCommand(householdId, userId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Title == "LLM Recipe 1");
        Assert.Contains(result, r => r.Title == "LLM Recipe 2");
    }

    [Fact]
    public async Task WhenLlmOutputInvalid_FallsBackToAdapterRecipes()
    {
        // Arrange
        await using var db = CreateDb();
        var (householdId, userId) = SeedDbWithItems(db);

        var adapterRecipes = new List<GeneratedRecipeDto>
        {
            new(Guid.NewGuid(), "Fallback Recipe 1",
                [new RecipeIngredientDto(Guid.NewGuid(), "Tomato", 3, "pcs")],
                ["Prepare.", "Serve."], null, null, 5, 10, 4),
            new(Guid.NewGuid(), "Fallback Recipe 2",
                [new RecipeIngredientDto(Guid.NewGuid(), "Pasta", 200, "g")],
                ["Cook.", "Plate."], null, null, 5, 15, 4)
        };

        var adapterMock = new Mock<IRecipeAdapter>();
        adapterMock.Setup(a => a.SearchAsync(
                It.IsAny<IList<string>>(),
                It.IsAny<RecipeConstraints>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(adapterRecipes);

        // LLM returns invalid output (empty ingredients/steps)
        var invalidLlmRecipes = new List<GeneratedRecipeDto>
        {
            new(Guid.NewGuid(), "Bad Recipe 1", [], [], null, null, 0, 0, 1),
            new(Guid.NewGuid(), "Bad Recipe 2", [], [], null, null, 0, 0, 1)
        };

        var llmMock = new Mock<ILlmService>();
        llmMock.Setup(s => s.GenerateRecipesAsync(
                It.IsAny<IList<Item>>(),
                It.IsAny<IList<GeneratedRecipeDto>>(),
                It.IsAny<RecipeConstraints>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidLlmRecipes);

        var rateLimiterMock = new Mock<IGenerationRateLimiter>();
        rateLimiterMock.Setup(r => r.TryConsume(It.IsAny<Guid>())).Returns(true);

        var handler = CreateHandler(db, llmMock.Object, adapterMock.Object, rateLimiterMock.Object);
        var command = new GenerateRecipeCommand(householdId, userId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert: fell back to adapter recipes
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Title == "Fallback Recipe 1");
        Assert.Contains(result, r => r.Title == "Fallback Recipe 2");
    }

    [Fact]
    public async Task WhenRateLimitExceeded_ThrowsRateLimitExceededException()
    {
        // Arrange
        await using var db = CreateDb();
        var (householdId, userId) = SeedDbWithItems(db);

        var rateLimiterMock = new Mock<IGenerationRateLimiter>();
        rateLimiterMock.Setup(r => r.TryConsume(It.IsAny<Guid>())).Returns(false);

        var llmMock = new Mock<ILlmService>();
        var adapterMock = new Mock<IRecipeAdapter>();

        var handler = CreateHandler(db, llmMock.Object, adapterMock.Object, rateLimiterMock.Object);
        var command = new GenerateRecipeCommand(householdId, userId);

        // Act & Assert
        await Assert.ThrowsAsync<RateLimitExceededException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
