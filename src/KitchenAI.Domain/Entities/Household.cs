namespace KitchenAI.Domain.Entities;

/// <summary>A household groups users and their shared inventory.</summary>
public class Household
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User Owner { get; set; } = null!;
    public ICollection<HouseholdMember> Members { get; set; } = [];
    public ICollection<Item> Items { get; set; } = [];
    public ICollection<Recipe> Recipes { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<AnalyticsEvent> AnalyticsEvents { get; set; } = [];
}
