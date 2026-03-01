using MediatR;

namespace KitchenAI.Application.Gdpr;

/// <summary>Permanently deletes the user and all their associated data.</summary>
public record DeleteUserCommand(Guid UserId) : IRequest;
