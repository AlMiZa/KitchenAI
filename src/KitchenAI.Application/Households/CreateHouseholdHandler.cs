using KitchenAI.Application.Persistence;
using KitchenAI.Domain.Entities;
using MediatR;

namespace KitchenAI.Application.Households;

/// <summary>Creates a new household and sets the requesting user as the owner.</summary>
public class CreateHouseholdHandler(IAppDbContext db) : IRequestHandler<CreateHouseholdCommand, HouseholdDto>
{
    /// <inheritdoc/>
    public async Task<HouseholdDto> Handle(CreateHouseholdCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);

        var household = new Household
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            OwnerUserId = request.UserId,
            CreatedAt = DateTime.UtcNow
        };

        var member = new HouseholdMember
        {
            Id = Guid.NewGuid(),
            HouseholdId = household.Id,
            UserId = request.UserId,
            Role = "owner"
        };

        db.Households.Add(household);
        db.HouseholdMembers.Add(member);
        await db.SaveChangesAsync(cancellationToken);

        return new HouseholdDto(household.Id, household.Name, household.OwnerUserId, 1);
    }
}
