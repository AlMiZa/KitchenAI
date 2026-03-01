using MediatR;

namespace KitchenAI.Application.Households;

/// <summary>Creates a new household owned by the requesting user.</summary>
public record CreateHouseholdCommand(Guid UserId, string Name) : IRequest<HouseholdDto>;
