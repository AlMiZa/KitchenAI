using KitchenAI.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Households;

/// <summary>Returns a single household the user belongs to.</summary>
public class GetHouseholdHandler(IAppDbContext db)
    : IRequestHandler<GetHouseholdQuery, HouseholdDto?>
{
    /// <inheritdoc/>
    public async Task<HouseholdDto?> Handle(GetHouseholdQuery request, CancellationToken cancellationToken)
    {
        // EF Core translates the Any() and Count() calls into correlated subqueries within a single
        // SQL statement — not N+1 — because this projection targets a single row via FirstOrDefaultAsync.
        var household = await db.Households
            .Where(h => h.Id == request.HouseholdId)
            .Where(h => h.OwnerUserId == request.UserId
                     || db.HouseholdMembers.Any(m => m.HouseholdId == h.Id && m.UserId == request.UserId))
            .Select(h => new HouseholdDto(
                h.Id,
                h.Name,
                h.OwnerUserId,
                db.HouseholdMembers.Count(m => m.HouseholdId == h.Id) + 1))
            .FirstOrDefaultAsync(cancellationToken);

        return household;
    }
}
