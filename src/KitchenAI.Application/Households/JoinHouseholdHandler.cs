using KitchenAI.Application.Persistence;
using KitchenAI.Application.Resources;
using KitchenAI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Households;

/// <summary>Adds the user as a member of an existing household.</summary>
public class JoinHouseholdHandler(IAppDbContext db) : IRequestHandler<JoinHouseholdCommand, HouseholdDto>
{
    /// <inheritdoc/>
    public async Task<HouseholdDto> Handle(JoinHouseholdCommand request, CancellationToken cancellationToken)
    {
        var household = await db.Households
            .Include(h => h.Members)
            .FirstOrDefaultAsync(h => h.Id == request.HouseholdId, cancellationToken)
            ?? throw new KeyNotFoundException(Messages.Get("Household_NotFound", request.HouseholdId));

        var alreadyMember = household.Members.Any(m => m.UserId == request.UserId);
        if (!alreadyMember)
        {
            db.HouseholdMembers.Add(new HouseholdMember
            {
                Id = Guid.NewGuid(),
                HouseholdId = household.Id,
                UserId = request.UserId,
                Role = "member"
            });
            await db.SaveChangesAsync(cancellationToken);
            // Reload member count after save
            household = await db.Households
                .Include(h => h.Members)
                .FirstAsync(h => h.Id == request.HouseholdId, cancellationToken);
        }

        return new HouseholdDto(household.Id, household.Name, household.OwnerUserId, household.Members.Count);
    }
}
