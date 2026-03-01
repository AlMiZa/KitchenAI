namespace KitchenAI.Application.Auth;

/// <summary>Returned after a successful registration or login.</summary>
public record AuthResultDto(
    string Token,
    Guid UserId,
    string Email,
    string DisplayName,
    Guid? HouseholdId);
