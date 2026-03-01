namespace KitchenAI.Domain.Entities;

/// <summary>Represents an application user.</summary>
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Locale { get; set; } = "pl-PL";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    /// <summary>JSON-serialised notification preferences.</summary>
    public string? NotificationPreferences { get; set; }

    /// <summary>JSON-serialised dietary preferences (allergies, diets, calorie target).</summary>
    public string? DietaryPreferences { get; set; }

    /// <summary>Application-level role: "user" or "admin".</summary>
    public string Role { get; set; } = "user";

    public ICollection<HouseholdMember> HouseholdMemberships { get; set; } = [];
}
