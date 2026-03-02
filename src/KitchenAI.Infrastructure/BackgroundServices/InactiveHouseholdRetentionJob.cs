using KitchenAI.Application.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KitchenAI.Infrastructure.BackgroundServices;

/// <summary>Runs the inactive household retention job on a daily schedule.</summary>
public class InactiveHouseholdRetentionService(
    IServiceScopeFactory scopeFactory,
    ILogger<InactiveHouseholdRetentionService> logger) : BackgroundService
{
    private static readonly TimeSpan InitialDelay = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan Interval = TimeSpan.FromHours(24);

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Inactive household retention service starting; first run in {Delay}.", InitialDelay);

        try
        {
            await Task.Delay(InitialDelay, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = scopeFactory.CreateScope();
                var job = scope.ServiceProvider.GetRequiredService<InactiveHouseholdRetentionJob>();
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

/// <summary>Core logic for removing households that have been inactive beyond the configured retention period.</summary>
public class InactiveHouseholdRetentionJob(
    IAppDbContext db,
    IConfiguration configuration,
    ILogger<InactiveHouseholdRetentionJob> logger)
{
    private const int DefaultRetentionMonths = 24;

    /// <summary>Deletes households whose last analytics event (or creation date, if no events exist) is older than the configured retention period.</summary>
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var retentionMonths = configuration.GetValue("DataRetention:InactiveHouseholdMonths", DefaultRetentionMonths);
        var cutoff = DateTime.UtcNow.AddMonths(-retentionMonths);

        logger.LogInformation(
            "Running inactive household retention check. Cutoff: {Cutoff} ({Months} months).",
            cutoff, retentionMonths);

        // Translated to SQL NOT EXISTS, which is efficient for the expected household scale.
        // Households active within the retention window are excluded by the correlated event check.
        var inactiveHouseholds = await db.Households
            .Where(h => h.CreatedAt < cutoff
                        && !db.AnalyticsEvents.Any(e => e.HouseholdId == h.Id && e.CreatedAt > cutoff))
            .ToListAsync(cancellationToken);

        if (inactiveHouseholds.Count == 0)
        {
            logger.LogInformation("No inactive households found for deletion.");
            return;
        }

        db.Households.RemoveRange(inactiveHouseholds);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogWarning(
            "Deleted {Count} inactive household(s) not active since {Cutoff}.",
            inactiveHouseholds.Count, cutoff);
    }
}
