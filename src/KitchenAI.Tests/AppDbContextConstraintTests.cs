using KitchenAI.Domain.Entities;
using KitchenAI.Domain.Enums;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Tests;

/// <summary>
/// Tests that verify schema constraints, cascade deletes, and query patterns
/// using a real SQLite in-memory database (EF InMemory provider does not enforce these).
/// </summary>
public class AppDbContextConstraintTests : IDisposable
{
    private readonly SqliteConnection _connection;

    public AppDbContextConstraintTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        using var ctx = CreateContext();
        ctx.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task User_email_unique_constraint_is_enforced()
    {
        await using var ctx = CreateContext();

        ctx.Users.Add(MakeUser("dup@kitchen.ai"));
        ctx.Users.Add(MakeUser("dup@kitchen.ai"));

        await Assert.ThrowsAsync<DbUpdateException>(() => ctx.SaveChangesAsync());
    }

    [Fact]
    public async Task Deleting_household_cascades_to_items()
    {
        await using var ctx = CreateContext();
        var (_, household) = await SeedHouseholdAsync(ctx);

        ctx.Items.Add(new Item
        {
            Id = Guid.NewGuid(),
            HouseholdId = household.Id,
            Name = "Milk",
            Quantity = 1,
            Unit = "L",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();

        ctx.Households.Remove(household);
        await ctx.SaveChangesAsync();

        await using var verify = CreateContext();
        Assert.Equal(0, await verify.Items.CountAsync(i => i.HouseholdId == household.Id));
    }

    [Fact]
    public async Task Deleting_household_cascades_to_notifications()
    {
        await using var ctx = CreateContext();
        var (_, household) = await SeedHouseholdAsync(ctx);

        ctx.Notifications.Add(new Notification
        {
            Id = Guid.NewGuid(),
            HouseholdId = household.Id,
            Type = NotificationType.Expiring,
            CreatedAt = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();

        ctx.Households.Remove(household);
        await ctx.SaveChangesAsync();

        await using var verify = CreateContext();
        Assert.Equal(0, await verify.Notifications.CountAsync(n => n.HouseholdId == household.Id));
    }

    [Fact]
    public async Task Items_can_be_filtered_by_household_and_expiry_date()
    {
        await using var ctx = CreateContext();
        var (_, household) = await SeedHouseholdAsync(ctx);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        ctx.Items.AddRange(
            new Item { Id = Guid.NewGuid(), HouseholdId = household.Id, Name = "Expired", Quantity = 1, Unit = "L", ExpiryDate = today.AddDays(-1), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Item { Id = Guid.NewGuid(), HouseholdId = household.Id, Name = "Fresh", Quantity = 1, Unit = "L", ExpiryDate = today.AddDays(7), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Item { Id = Guid.NewGuid(), HouseholdId = household.Id, Name = "No Date", Quantity = 1, Unit = "L", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );
        await ctx.SaveChangesAsync();

        await using var verify = CreateContext();
        var expiring = await verify.Items
            .Where(i => i.HouseholdId == household.Id && i.ExpiryDate != null && i.ExpiryDate <= today.AddDays(3))
            .OrderBy(i => i.ExpiryDate)
            .ToListAsync();

        Assert.Single(expiring);
        Assert.Equal("Expired", expiring[0].Name);
    }

    private static User MakeUser(string email) => new()
    {
        Id = Guid.NewGuid(),
        Email = email,
        PasswordHash = "hash",
        DisplayName = "Test",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    private static async Task<(User Owner, Household Household)> SeedHouseholdAsync(AppDbContext ctx)
    {
        var owner = MakeUser($"owner-{Guid.NewGuid()}@kitchen.ai");
        ctx.Users.Add(owner);

        var household = new Household
        {
            Id = Guid.NewGuid(),
            Name = "Test Household",
            OwnerUserId = owner.Id,
            CreatedAt = DateTime.UtcNow
        };
        ctx.Households.Add(household);
        await ctx.SaveChangesAsync();

        return (owner, household);
    }
}
