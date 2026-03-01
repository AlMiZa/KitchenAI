using MediatR;

namespace KitchenAI.Application.Notifications;

/// <summary>Updates the user's notification subscription preferences.</summary>
public record SubscribeNotificationsCommand(
    Guid HouseholdId,
    Guid UserId,
    bool EmailEnabled,
    bool PushEnabled,
    int ExpiryThresholdDays) : IRequest;
