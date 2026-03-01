namespace KitchenAI.Domain.Entities;

/// <summary>Represents a user's membership in a household with an assigned role.</summary>
public class HouseholdMember
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public Guid UserId { get; set; }

    /// <summary>Role within the household: "owner" or "member".</summary>
    public string Role { get; set; } = "member";

    public Household Household { get; set; } = null!;
    public User User { get; set; } = null!;
}
