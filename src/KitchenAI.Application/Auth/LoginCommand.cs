using MediatR;

namespace KitchenAI.Application.Auth;

/// <summary>Authenticates an existing user by email and password.</summary>
public record LoginCommand(string Email, string Password) : IRequest<AuthResultDto>;
