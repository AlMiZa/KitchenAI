using MediatR;

namespace KitchenAI.Application.Notifications;

/// <summary>Creates a test notification for the specified household.</summary>
public record CreateTestNotificationCommand(Guid HouseholdId) : IRequest<NotificationDto>;
