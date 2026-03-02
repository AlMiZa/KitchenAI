using KitchenAI.Application.Items;
using KitchenAI.Domain.Entities;
using KitchenAI.Domain.Enums;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Tests.Items;

public class GetItemsHandlerTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task ExpiringSoonFilter_ReturnsSortedByExpiryAscending()
    {
        // Arrange
        await using var db = CreateDb();
        var householdId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        db.Items.AddRange(
            new Item
            {
                Id = Guid.NewGuid(),
                HouseholdId = householdId,
                Name = "Yogurt",
                Quantity = 1,
                Unit = "pcs",
                ExpiryDate = DateOnly.FromDateTime(now.AddDays(3)),
                CreatedAt = now,
                UpdatedAt = now
            },
            new Item
            {
                Id = Guid.NewGuid(),
                HouseholdId = householdId,
                Name = "Milk",
                Quantity = 1,
                Unit = "L",
                ExpiryDate = DateOnly.FromDateTime(now.AddDays(1)),
                CreatedAt = now,
                UpdatedAt = now
            },
            new Item
            {
                Id = Guid.NewGuid(),
                HouseholdId = householdId,
                Name = "Pasta",
                Quantity = 1,
                Unit = "kg",
                ExpiryDate = DateOnly.FromDateTime(now.AddDays(30)),
                CreatedAt = now,
                UpdatedAt = now
            });
        await db.SaveChangesAsync();

        var handler = new GetItemsHandler(db);
        var query = new GetItemsQuery(householdId, ExpiringSoon: true, ExpiryThresholdDays: 7);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert – only items expiring within 7 days, sorted ascending
        Assert.Equal(2, result.Count);
        Assert.Equal("Milk", result[0].Name);
        Assert.Equal("Yogurt", result[1].Name);
    }
}
