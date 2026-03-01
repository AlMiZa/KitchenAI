using MediatR;

namespace KitchenAI.Application.Auth;

/// <summary>Registers a new user account and creates their default household.</summary>
public record RegisterCommand(string Email, string Password, string DisplayName) : IRequest<AuthResultDto>;
