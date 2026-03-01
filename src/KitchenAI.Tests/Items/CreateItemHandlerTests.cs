using KitchenAI.Application.Items;
using KitchenAI.Application.Exceptions;
using KitchenAI.Domain.Enums;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Tests.Items;

public class CreateItemHandlerTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task ValidItem_PersistedCorrectly()
    {
        // Arrange
        await using var db = CreateDb();
        var handler = new CreateItemHandler(db);
        var householdId = Guid.NewGuid();
        var command = new CreateItemCommand(
            HouseholdId: householdId,
            Name: "Milk",
            Quantity: 2,
            Unit: "L",
            AllowFraction: true,
            PurchaseDate: DateOnly.FromDateTime(DateTime.Today),
            ExpiryDate: DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
            BestByOrUseBy: BestByOrUseBy.UseBy,
            StorageLocation: StorageLocation.Fridge,
            Brand: "FreshFarm",
            Price: 3.50m,
            Notes: "Organic");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Milk", result.Name);
        Assert.Equal(2, result.Quantity);
        Assert.Equal("L", result.Unit);
        Assert.Equal(StorageLocation.Fridge, result.StorageLocation);
        Assert.Equal(3.50m, result.Price);
        Assert.False(result.IsArchived);

        var saved = await db.Items.FindAsync(result.Id);
        Assert.NotNull(saved);
        Assert.Equal("Milk", saved.Name);
    }

    [Fact]
    public async Task MissingName_ThrowsArgumentException()
    {
        // Arrange
        await using var db = CreateDb();
        var handler = new CreateItemHandler(db);
        var command = new CreateItemCommand(
            HouseholdId: Guid.NewGuid(),
            Name: "",
            Quantity: 1,
            Unit: "pcs",
            AllowFraction: false,
            PurchaseDate: null,
            ExpiryDate: null,
            BestByOrUseBy: null,
            StorageLocation: StorageLocation.Pantry,
            Brand: null,
            Price: 0,
            Notes: null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}
