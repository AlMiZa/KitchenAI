using KitchenAI.Domain.Entities;
using KitchenAI.Domain.Enums;
using KitchenAI.Infrastructure.BackgroundServices;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace KitchenAI.Tests.Notifications;

public class ExpiryNotificationServiceTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task ItemsWithinThreshold_NotificationsCreated()
    {
        // Arrange
        await using var db = CreateDb();
        var householdId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        db.Items.AddRange(
            new Item
            {
                Id = Guid.NewGuid(), HouseholdId = householdId,
                Name = "Yogurt", Quantity = 1, Unit = "pcs",
                ExpiryDate = DateOnly.FromDateTime(now.AddDays(1)),
                CreatedAt = now, UpdatedAt = now
            },
            new Item
            {
                Id = Guid.NewGuid(), HouseholdId = householdId,
                Name = "Cheese", Quantity = 1, Unit = "pcs",
                ExpiryDate = DateOnly.FromDateTime(now.AddDays(2)),
                CreatedAt = now, UpdatedAt = now
            });
        await db.SaveChangesAsync();

        var job = new ExpiryNotificationJob(db, NullLogger<ExpiryNotificationJob>.Instance);

        // Act
        await job.RunAsync();

        // Assert
        var notifications = await db.Notifications.ToListAsync();
        Assert.Equal(2, notifications.Count);
        Assert.All(notifications, n => Assert.Equal(NotificationType.Expiring, n.Type));
        Assert.All(notifications, n => Assert.False(n.Delivered));
    }

    [Fact]
    public async Task SecondRun_DoesNotDuplicateNotifications()
    {
        // Arrange
        await using var db = CreateDb();
        var householdId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        db.Items.Add(new Item
        {
            Id = Guid.NewGuid(), HouseholdId = householdId,
            Name = "Milk", Quantity = 1, Unit = "L",
            ExpiryDate = DateOnly.FromDateTime(now.AddDays(1)),
            CreatedAt = now, UpdatedAt = now
        });
        await db.SaveChangesAsync();

        var job = new ExpiryNotificationJob(db, NullLogger<ExpiryNotificationJob>.Instance);

        // Act - run twice to simulate two consecutive daily executions
        await job.RunAsync();
        await job.RunAsync();

        // Assert – still only one notification per item
        var count = await db.Notifications.CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task AfterNotificationDelivered_NewNotificationCreatedOnNextRun()
    {
        // Arrange
        await using var db = CreateDb();
        var householdId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var itemId = Guid.NewGuid();

        db.Items.Add(new Item
        {
            Id = itemId, HouseholdId = householdId,
            Name = "Butter", Quantity = 1, Unit = "pcs",
            ExpiryDate = DateOnly.FromDateTime(now.AddDays(1)),
            CreatedAt = now, UpdatedAt = now
        });
        await db.SaveChangesAsync();

        var job = new ExpiryNotificationJob(db, NullLogger<ExpiryNotificationJob>.Instance);

        // First run creates the notification
        await job.RunAsync();
        Assert.Equal(1, await db.Notifications.CountAsync());

        // Mark the existing notification as delivered
        var existing = await db.Notifications.SingleAsync();
        existing.Delivered = true;
        await db.SaveChangesAsync();

        // Second run should create a fresh notification since the previous one was delivered
        await job.RunAsync();
        Assert.Equal(2, await db.Notifications.CountAsync());
    }

    [Fact]
    public async Task ArchivedItems_NoNotificationsCreated()
    {
        // Arrange
        await using var db = CreateDb();
        var householdId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        db.Items.Add(new Item
        {
            Id = Guid.NewGuid(), HouseholdId = householdId,
            Name = "Old Milk", Quantity = 1, Unit = "L",
            ExpiryDate = DateOnly.FromDateTime(now.AddDays(1)),
            IsArchived = true,
            CreatedAt = now, UpdatedAt = now
        });
        await db.SaveChangesAsync();

        var job = new ExpiryNotificationJob(db, NullLogger<ExpiryNotificationJob>.Instance);

        // Act
        await job.RunAsync();

        // Assert
        var count = await db.Notifications.CountAsync();
        Assert.Equal(0, count);
    }
}
