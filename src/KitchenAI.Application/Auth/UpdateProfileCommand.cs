using MediatR;

namespace KitchenAI.Application.Auth;

/// <summary>Updates a user's display name and/or locale.</summary>
public record UpdateProfileCommand(
    Guid UserId,
    string? DisplayName,
    string? Locale) : IRequest<UserDto>;
