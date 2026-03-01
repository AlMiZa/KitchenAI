using MediatR;

namespace KitchenAI.Application.Households;

/// <summary>Adds the requesting user as a member of the specified household.</summary>
public record JoinHouseholdCommand(Guid UserId, Guid HouseholdId) : IRequest<HouseholdDto>;
