namespace KitchenAI.Application.Auth;

/// <summary>Profile of the current authenticated user.</summary>
public record UserDto(
    Guid Id,
    string Email,
    string DisplayName,
    string Locale,
    Guid? HouseholdId);
