using KitchenAI.Application.Recipes;
using KitchenAI.Application.Services;
using KitchenAI.Domain.Entities;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace KitchenAI.Tests.Recipes;

public class GenerateRecipeHandlerTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task WithValidItems_ReturnsTwoRecipes()
    {
        // Arrange
        await using var db = CreateDb();
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
                Id = Guid.NewGuid(), HouseholdId = householdId,
                Name = "Tomato", Quantity = 3, Unit = "pcs",
                CreatedAt = now, UpdatedAt = now
            },
            new Item
            {
                Id = Guid.NewGuid(), HouseholdId = householdId,
                Name = "Pasta", Quantity = 500, Unit = "g",
                CreatedAt = now, UpdatedAt = now
            });
        await db.SaveChangesAsync();

        var llmMock = new Mock<ILlmService>();
        llmMock.Setup(s => s.GenerateRecipesAsync(
                It.IsAny<IList<Item>>(),
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

        var handler = new GenerateRecipeHandler(db, llmMock.Object);
        var command = new GenerateRecipeCommand(householdId, userId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Count >= 2);
        Assert.Contains(result, r => r.Title == "Tomato Pasta");
        Assert.Contains(result, r => r.Title == "Pasta Salad");
        Assert.Equal(1, await db.GeneratedRecipes.CountAsync());
    }
}
