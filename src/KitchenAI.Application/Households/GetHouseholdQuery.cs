using MediatR;

namespace KitchenAI.Application.Households;

/// <summary>Returns a single household by its identifier.</summary>
public record GetHouseholdQuery(Guid UserId, Guid HouseholdId) : IRequest<HouseholdDto?>;
