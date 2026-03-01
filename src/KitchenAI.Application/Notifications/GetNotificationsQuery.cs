using MediatR;

namespace KitchenAI.Application.Notifications;

/// <summary>Returns undelivered notifications for the specified household.</summary>
public record GetNotificationsQuery(Guid HouseholdId) : IRequest<IList<NotificationDto>>;
