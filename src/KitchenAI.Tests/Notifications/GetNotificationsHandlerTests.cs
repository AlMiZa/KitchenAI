using KitchenAI.Application.Notifications;
using KitchenAI.Domain.Entities;
using KitchenAI.Domain.Enums;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Tests.Notifications;

public class GetNotificationsHandlerTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Handle_ReturnsOnlyUndeliveredNotificationsForHousehold()
    {
        // Arrange
        await using var db = CreateDb();
        var householdId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        db.Notifications.AddRange(
            new Notification
            {
                Id = Guid.NewGuid(),
                HouseholdId = householdId,
                Type = NotificationType.Expiring,
                Payload = """{"item":"Milk"}""",
                Delivered = false,
                CreatedAt = now
            },
            new Notification
            {
                Id = Guid.NewGuid(),
                HouseholdId = householdId,
                Type = NotificationType.LowStock,
                Payload = """{"item":"Eggs"}""",
                Delivered = false,
                CreatedAt = now.AddMinutes(-5)
            });
        await db.SaveChangesAsync();

        var handler = new GetNotificationsHandler(db);

        // Act
        var result = await handler.Handle(new GetNotificationsQuery(householdId), CancellationToken.None);

        // Assert – both undelivered notifications are returned
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_ExcludesDeliveredNotifications()
    {
        // Arrange
        await using var db = CreateDb();
        var householdId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        db.Notifications.AddRange(
            new Notification
            {
                Id = Guid.NewGuid(),
                HouseholdId = householdId,
                Type = NotificationType.Expiring,
                Payload = """{"item":"Yogurt"}""",
                Delivered = false,
                CreatedAt = now
            },
            new Notification
            {
                Id = Guid.NewGuid(),
                HouseholdId = householdId,
                Type = NotificationType.Expiring,
                Payload = """{"item":"Butter"}""",
                Delivered = true,
                CreatedAt = now.AddHours(-1)
            });
        await db.SaveChangesAsync();

        var handler = new GetNotificationsHandler(db);

        // Act
        var result = await handler.Handle(new GetNotificationsQuery(householdId), CancellationToken.None);

        // Assert – only the undelivered notification is returned
        Assert.Single(result);
        Assert.False(result[0].Delivered);
    }

    [Fact]
    public async Task Handle_ExcludesNotificationsFromOtherHouseholds()
    {
        // Arrange
        await using var db = CreateDb();
        var targetHouseholdId = Guid.NewGuid();
        var otherHouseholdId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        db.Notifications.AddRange(
            new Notification
            {
                Id = Guid.NewGuid(),
                HouseholdId = targetHouseholdId,
                Type = NotificationType.RecipeSuggestion,
                Payload = null,
                Delivered = false,
                CreatedAt = now
            },
            new Notification
            {
                Id = Guid.NewGuid(),
                HouseholdId = otherHouseholdId,
                Type = NotificationType.Expiring,
                Payload = null,
                Delivered = false,
                CreatedAt = now
            });
        await db.SaveChangesAsync();

        var handler = new GetNotificationsHandler(db);

        // Act
        var result = await handler.Handle(new GetNotificationsQuery(targetHouseholdId), CancellationToken.None);

        // Assert – only the notification for the target household is returned
        Assert.Single(result);
        Assert.Equal(NotificationType.RecipeSuggestion, result[0].Type);
    }

    [Fact]
    public async Task Handle_ReturnsNotificationsSortedByCreatedAtDescending()
    {
        // Arrange
        await using var db = CreateDb();
        var householdId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var olderId = Guid.NewGuid();
        var newerId = Guid.NewGuid();

        db.Notifications.AddRange(
            new Notification
            {
                Id = olderId,
                HouseholdId = householdId,
                Type = NotificationType.Expiring,
                Payload = null,
                Delivered = false,
                CreatedAt = now.AddMinutes(-10)
            },
            new Notification
            {
                Id = newerId,
                HouseholdId = householdId,
                Type = NotificationType.LowStock,
                Payload = null,
                Delivered = false,
                CreatedAt = now
            });
        await db.SaveChangesAsync();

        var handler = new GetNotificationsHandler(db);

        // Act
        var result = await handler.Handle(new GetNotificationsQuery(householdId), CancellationToken.None);

        // Assert – newest notification comes first
        Assert.Equal(2, result.Count);
        Assert.Equal(newerId, result[0].Id);
        Assert.Equal(olderId, result[1].Id);
    }
}
