using KitchenAI.Application.Items;
using KitchenAI.Domain.Entities;
using KitchenAI.Domain.Enums;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Tests.Items;

public class MergeItemsHandlerTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task CombinesTwoItems_SumsQuantityAndArchivesSecond()
    {
        // Arrange
        await using var db = CreateDb();
        var householdId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var item1 = new Item
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            Name = "Milk",
            Quantity = 1.5m,
            Unit = "L",
            StorageLocation = StorageLocation.Fridge,
            CreatedAt = now,
            UpdatedAt = now
        };
        var item2 = new Item
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            Name = "Milk",
            Quantity = 0.5m,
            Unit = "L",
            StorageLocation = StorageLocation.Fridge,
            CreatedAt = now.AddSeconds(1),
            UpdatedAt = now.AddSeconds(1)
        };

        db.Items.AddRange(item1, item2);
        await db.SaveChangesAsync();

        var handler = new MergeItemsHandler(db);
        var command = new MergeItemsCommand(householdId, [item1.Id, item2.Id]);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert – primary item has combined quantity
        Assert.Equal(2.0m, result.Quantity);
        Assert.Equal(item1.Id, result.Id);
        Assert.False(result.IsArchived);

        // Secondary item is archived
        var archived = await db.Items.FindAsync(item2.Id);
        Assert.NotNull(archived);
        Assert.True(archived.IsArchived);
    }

    [Fact]
    public async Task SingleItemId_ThrowsArgumentException()
    {
        // Arrange
        await using var db = CreateDb();
        var handler = new MergeItemsHandler(db);
        var command = new MergeItemsCommand(Guid.NewGuid(), [Guid.NewGuid()]);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task ItemsNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        await using var db = CreateDb();
        var handler = new MergeItemsHandler(db);
        var command = new MergeItemsCommand(Guid.NewGuid(), [Guid.NewGuid(), Guid.NewGuid()]);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}
