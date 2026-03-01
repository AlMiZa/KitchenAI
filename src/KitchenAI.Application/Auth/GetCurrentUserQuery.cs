using MediatR;

namespace KitchenAI.Application.Auth;

/// <summary>Retrieves profile information for the currently authenticated user.</summary>
public record GetCurrentUserQuery(Guid UserId) : IRequest<UserDto>;
