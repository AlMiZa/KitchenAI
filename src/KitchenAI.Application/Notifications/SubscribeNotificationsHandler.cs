using System.Text.Json;
using KitchenAI.Application.Persistence;
using MediatR;

namespace KitchenAI.Application.Notifications;

/// <summary>Updates notification preferences for the requesting user.</summary>
public class SubscribeNotificationsHandler(IAppDbContext db)
    : IRequestHandler<SubscribeNotificationsCommand>
{
    private sealed record NotificationPrefs(bool EmailEnabled, bool PushEnabled, int ExpiryThresholdDays);

    /// <inheritdoc/>
    public async Task Handle(SubscribeNotificationsCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FindAsync([request.UserId], cancellationToken)
            ?? throw new KeyNotFoundException($"User {request.UserId} not found.");

        user.NotificationPreferences = JsonSerializer.Serialize(
            new NotificationPrefs(request.EmailEnabled, request.PushEnabled, request.ExpiryThresholdDays));

        await db.SaveChangesAsync(cancellationToken);
    }
}
