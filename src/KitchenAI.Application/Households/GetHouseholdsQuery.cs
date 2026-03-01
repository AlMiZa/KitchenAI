using MediatR;

namespace KitchenAI.Application.Households;

/// <summary>Returns all households the user belongs to.</summary>
public record GetHouseholdsQuery(Guid UserId) : IRequest<IList<HouseholdDto>>;
