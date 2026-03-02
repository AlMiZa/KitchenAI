using KitchenAI.Application.Persistence;
using KitchenAI.Application.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Auth;

/// <summary>Returns profile information for the current user.</summary>
public class GetCurrentUserHandler(IAppDbContext db) : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    /// <inheritdoc/>
    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FindAsync([request.UserId], cancellationToken)
            ?? throw new UnauthorizedAccessException(Messages.Get("Auth_UserNotFound"));

        var householdId = await db.Households
            .Where(h => h.OwnerUserId == user.Id)
            .Select(h => (Guid?)h.Id)
            .FirstOrDefaultAsync(cancellationToken)
            ?? await db.HouseholdMembers
                .Where(m => m.UserId == user.Id)
                .Select(m => (Guid?)m.HouseholdId)
                .FirstOrDefaultAsync(cancellationToken);

        return new UserDto(user.Id, user.Email, user.DisplayName, user.Locale, householdId, user.Role);
    }
}
