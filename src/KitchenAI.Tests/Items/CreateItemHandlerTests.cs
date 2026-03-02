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
        Assert.True(result.AllowFraction);
        Assert.Equal(command.PurchaseDate, result.PurchaseDate);
        Assert.Equal(command.ExpiryDate, result.ExpiryDate);
        Assert.Equal(BestByOrUseBy.UseBy, result.BestByOrUseBy);
        Assert.Equal(StorageLocation.Fridge, result.StorageLocation);
        Assert.Equal("FreshFarm", result.Brand);
        Assert.Equal(3.50m, result.Price);
        Assert.Equal("Organic", result.Notes);
        Assert.False(result.IsArchived);

        var saved = await db.Items.FindAsync(result.Id);
        Assert.NotNull(saved);
        Assert.Equal("Milk", saved.Name);
    }

    [Fact]
    public async Task FractionalQuantity_StoredWithoutRounding()
    {
        // Arrange
        await using var db = CreateDb();
        var handler = new CreateItemHandler(db);
        var command = new CreateItemCommand(
            HouseholdId: Guid.NewGuid(),
            Name: "Flour",
            Quantity: 0.5m,
            Unit: "kg",
            AllowFraction: true,
            PurchaseDate: null,
            ExpiryDate: null,
            BestByOrUseBy: null,
            StorageLocation: StorageLocation.Pantry,
            Brand: null,
            Price: 1.20m,
            Notes: null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(0.5m, result.Quantity);
        var saved = await db.Items.FindAsync(result.Id);
        Assert.NotNull(saved);
        Assert.Equal(0.5m, saved.Quantity);
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
