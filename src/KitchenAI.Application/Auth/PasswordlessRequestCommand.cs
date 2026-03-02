using MediatR;

namespace KitchenAI.Application.Auth;

/// <summary>Requests a passwordless magic-link to be sent to the given email address.</summary>
public record PasswordlessRequestCommand(string Email) : IRequest;
