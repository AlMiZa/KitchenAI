using KitchenAI.Domain.Enums;

namespace KitchenAI.Domain.Entities;

/// <summary>An in-app notification for a household.</summary>
public class Notification
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public NotificationType Type { get; set; }

    /// <summary>JSON-serialised notification payload.</summary>
    public string? Payload { get; set; }
    public bool Delivered { get; set; }
    public DateTime CreatedAt { get; set; }

    public Household Household { get; set; } = null!;
}
