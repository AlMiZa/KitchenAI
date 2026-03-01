using KitchenAI.Domain.Enums;

namespace KitchenAI.Application.Notifications;

/// <summary>Notification data-transfer object.</summary>
public record NotificationDto(
    Guid Id,
    NotificationType Type,
    string? Payload,
    bool Delivered,
    DateTime CreatedAt);
