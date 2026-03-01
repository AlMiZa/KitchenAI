using KitchenAI.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Households;

/// <summary>Returns all households the user is a member of.</summary>
public class GetHouseholdsHandler(IAppDbContext db) : IRequestHandler<GetHouseholdsQuery, IList<HouseholdDto>>
{
    /// <inheritdoc/>
    public async Task<IList<HouseholdDto>> Handle(GetHouseholdsQuery request, CancellationToken cancellationToken)
    {
        var households = await db.HouseholdMembers
            .Where(m => m.UserId == request.UserId)
            .Select(m => m.Household)
            .Include(h => h.Members)
            .ToListAsync(cancellationToken);

        return households
            .Select(h => new HouseholdDto(h.Id, h.Name, h.OwnerUserId, h.Members.Count))
            .ToList();
    }
}
