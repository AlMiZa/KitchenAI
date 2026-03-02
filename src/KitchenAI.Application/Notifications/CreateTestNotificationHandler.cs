using KitchenAI.Application.Persistence;
using KitchenAI.Domain.Entities;
using KitchenAI.Domain.Enums;
using MediatR;

namespace KitchenAI.Application.Notifications;

/// <summary>Handles creation of a test notification.</summary>
public class CreateTestNotificationHandler(IAppDbContext db) : IRequestHandler<CreateTestNotificationCommand, NotificationDto>
{
    /// <inheritdoc/>
    public async Task<NotificationDto> Handle(CreateTestNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            HouseholdId = request.HouseholdId,
            Type = NotificationType.RecipeSuggestion,
            Payload = """{"message":"This is a test notification."}""",
            Delivered = false,
            CreatedAt = DateTime.UtcNow
        };

        db.Notifications.Add(notification);
        await db.SaveChangesAsync(cancellationToken);

        return new NotificationDto(notification.Id, notification.Type, notification.Payload, notification.Delivered, notification.CreatedAt);
    }
}
