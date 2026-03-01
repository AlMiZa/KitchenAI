using KitchenAI.Domain.Entities;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Tests;

public class AppDbContextTests
{
    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CanAddAndRetrieveUser()
    {
        await using var context = CreateInMemoryContext();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@kitchen.ai",
            PasswordHash = "hashed",
            DisplayName = "Test User",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var retrieved = await context.Users.FindAsync(user.Id);

        Assert.NotNull(retrieved);
        Assert.Equal("test@kitchen.ai", retrieved.Email);
    }

    [Fact]
    public async Task CanAddAndRetrieveHousehold()
    {
        await using var context = CreateInMemoryContext();

        var owner = new User
        {
            Id = Guid.NewGuid(),
            Email = "owner@kitchen.ai",
            PasswordHash = "hashed",
            DisplayName = "Owner",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Users.Add(owner);

        var household = new Household
        {
            Id = Guid.NewGuid(),
            Name = "Smith Family",
            OwnerUserId = owner.Id,
            CreatedAt = DateTime.UtcNow
        };
        context.Households.Add(household);
        await context.SaveChangesAsync();

        var retrieved = await context.Households.FindAsync(household.Id);

        Assert.NotNull(retrieved);
        Assert.Equal("Smith Family", retrieved.Name);
    }

    [Fact]
    public async Task CanAddItemToHousehold()
    {
        await using var context = CreateInMemoryContext();

        var owner = new User
        {
            Id = Guid.NewGuid(),
            Email = "owner2@kitchen.ai",
            PasswordHash = "hashed",
            DisplayName = "Owner2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Users.Add(owner);

        var household = new Household
        {
            Id = Guid.NewGuid(),
            Name = "Test Household",
            OwnerUserId = owner.Id,
            CreatedAt = DateTime.UtcNow
        };
        context.Households.Add(household);

        var item = new Item
        {
            Id = Guid.NewGuid(),
            HouseholdId = household.Id,
            Name = "Milk",
            Quantity = 1,
            Unit = "L",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Items.Add(item);
        await context.SaveChangesAsync();

        var count = await context.Items.CountAsync(i => i.HouseholdId == household.Id);

        Assert.Equal(1, count);
    }
}
