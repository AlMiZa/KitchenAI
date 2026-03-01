namespace KitchenAI.Domain.Entities;

/// <summary>Append-only event record used for household analytics.</summary>
public class AnalyticsEvent
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }

    /// <summary>Event type string: item_added, item_removed, item_consumed, recipe_generated, recipe_saved, recipe_cooked.</summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>JSON-serialised event metadata.</summary>
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }

    public Household Household { get; set; } = null!;
}
