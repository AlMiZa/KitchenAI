using KitchenAI.Application.Persistence;
using KitchenAI.Domain.Entities;
using KitchenAI.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KitchenAI.Infrastructure.BackgroundServices;

/// <summary>Runs the expiry notification check on a nightly schedule.</summary>
public class ExpiryNotificationService(
    IServiceScopeFactory scopeFactory,
    ILogger<ExpiryNotificationService> logger) : BackgroundService
{
    private static readonly TimeSpan InitialDelay = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan Interval = TimeSpan.FromHours(24);

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Expiry notification service starting; first run in {Delay}.", InitialDelay);

        try
        {
            await Task.Delay(InitialDelay, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = scopeFactory.CreateScope();
                var job = scope.ServiceProvider.GetRequiredService<ExpiryNotificationJob>();
                await job.RunAsync(stoppingToken);

                await Task.Delay(Interval, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown
        }
    }
}

/// <summary>Core logic for creating expiry notifications; injectable and directly testable.</summary>
public class ExpiryNotificationJob(IAppDbContext db, ILogger<ExpiryNotificationJob> logger)
{
    private const int DefaultThresholdDays = 3;

    /// <summary>Parses the item ID from a notification payload, returning <see langword="null"/> on failure.</summary>
    private Guid? TryGetItemIdFromPayload(Notification notification)
    {
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(notification.Payload ?? "{}");

            return doc.RootElement.TryGetProperty("itemId", out var prop)
                ? prop.GetGuid()
                : null;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not parse itemId from notification {NotificationId} payload.", notification.Id);
            return null;
        }
    }

    /// <summary>Creates <see cref="Notification"/> records for items expiring within the threshold.</summary>
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var threshold = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(DefaultThresholdDays));
        logger.LogInformation("Checking for items expiring before {Threshold}.", threshold);

        var expiringItems = await db.Items
            .Where(i => !i.IsArchived
                        && i.ExpiryDate.HasValue
                        && i.ExpiryDate.Value <= threshold)
            .ToListAsync(cancellationToken);

        if (expiringItems.Count == 0)
        {
            logger.LogInformation("No expiring items found.");
            return;
        }

        // Determine which items already have an undelivered expiry notification so we don't create duplicates.
        var pendingNotifications = await db.Notifications
            .Where(n => n.Type == NotificationType.Expiring && !n.Delivered)
            .ToListAsync(cancellationToken);

        var alreadyNotifiedItemIds = pendingNotifications
            .Select(TryGetItemIdFromPayload)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToHashSet();

        // Create one notification per expiring item that does not yet have a pending notification.
        var notifications = expiringItems
            .Where(item => !alreadyNotifiedItemIds.Contains(item.Id))
            .Select(item => new Notification
            {
                Id = Guid.NewGuid(),
                HouseholdId = item.HouseholdId,
                Type = NotificationType.Expiring,
                Payload = System.Text.Json.JsonSerializer.Serialize(new
                {
                    itemId = item.Id,
                    itemName = item.Name,
                    expiryDate = item.ExpiryDate
                }),
                Delivered = false,
                CreatedAt = DateTime.UtcNow
            })
            .ToList();

        if (notifications.Count == 0)
        {
            logger.LogInformation("All expiring items already have a pending notification — nothing to create.");
            return;
        }

        db.Notifications.AddRange(notifications);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created {Count} expiry notification(s).", notifications.Count);
    }
}
