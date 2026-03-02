using KitchenAI.Application.Persistence;
using KitchenAI.Application.Resources;
using MediatR;

namespace KitchenAI.Application.Auth;

/// <summary>Updates display name and locale for the current user.</summary>
public class UpdateProfileHandler(IAppDbContext db) : IRequestHandler<UpdateProfileCommand, UserDto>
{
    /// <inheritdoc/>
    public async Task<UserDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FindAsync([request.UserId], cancellationToken)
            ?? throw new UnauthorizedAccessException(Messages.Get("Auth_UserNotFound"));

        if (request.DisplayName is not null)
        {
            user.DisplayName = request.DisplayName;
        }

        if (request.Locale is not null)
        {
            user.Locale = request.Locale;
        }

        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return new UserDto(user.Id, user.Email, user.DisplayName, user.Locale, HouseholdId: null);
    }
}
