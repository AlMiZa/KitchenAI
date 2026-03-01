using MediatR;

namespace KitchenAI.Application.Gdpr;

/// <summary>Exports all data associated with the user as a JSON string.</summary>
public record ExportUserDataQuery(Guid UserId) : IRequest<string>;
