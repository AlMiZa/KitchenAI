using KitchenAI.Domain.Entities;
using KitchenAI.Infrastructure.BackgroundServices;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace KitchenAI.Tests.Admin;

public class InactiveHouseholdRetentionJobTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static IConfiguration BuildConfig(int months = 24) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DataRetention:InactiveHouseholdMonths"] = months.ToString()
            })
            .Build();

    [Fact]
    public async Task HouseholdsInactiveLongerThanRetentionPeriod_AreDeleted()
    {
        await using var db = CreateDb();

        var userId = Guid.NewGuid();
        db.Users.Add(new User
        {
            Id = userId,
            Email = "old@example.com",
            PasswordHash = "hash",
            DisplayName = "Old User",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        var householdId = Guid.NewGuid();
        db.Households.Add(new Household
        {
            Id = householdId,
            Name = "Inactive Home",
            OwnerUserId = userId,
            CreatedAt = DateTime.UtcNow.AddMonths(-30)
        });

        db.AnalyticsEvents.Add(new AnalyticsEvent
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            EventType = "item_added",
            CreatedAt = DateTime.UtcNow.AddMonths(-30)
        });

        await db.SaveChangesAsync();

        var job = new InactiveHouseholdRetentionJob(db, BuildConfig(24), NullLogger<InactiveHouseholdRetentionJob>.Instance);
        await job.RunAsync();

        Assert.Equal(0, await db.Households.CountAsync());
    }

    [Fact]
    public async Task HouseholdsActiveWithinRetentionPeriod_AreNotDeleted()
    {
        await using var db = CreateDb();

        var userId = Guid.NewGuid();
        db.Users.Add(new User
        {
            Id = userId,
            Email = "active@example.com",
            PasswordHash = "hash",
            DisplayName = "Active User",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        var householdId = Guid.NewGuid();
        db.Households.Add(new Household
        {
            Id = householdId,
            Name = "Active Home",
            OwnerUserId = userId,
            CreatedAt = DateTime.UtcNow.AddMonths(-30)
        });

        db.AnalyticsEvents.Add(new AnalyticsEvent
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            EventType = "item_added",
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        });

        await db.SaveChangesAsync();

        var job = new InactiveHouseholdRetentionJob(db, BuildConfig(24), NullLogger<InactiveHouseholdRetentionJob>.Instance);
        await job.RunAsync();

        Assert.Equal(1, await db.Households.CountAsync());
    }
}
