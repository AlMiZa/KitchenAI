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

        // Group by household and create one notification per expiring item
        var notifications = expiringItems
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

        db.Notifications.AddRange(notifications);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created {Count} expiry notifications.", notifications.Count);
    }
}
