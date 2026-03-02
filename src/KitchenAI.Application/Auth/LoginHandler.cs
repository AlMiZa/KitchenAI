using KitchenAI.Application.Persistence;
using KitchenAI.Application.Resources;
using KitchenAI.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KitchenAI.Application.Auth;

/// <summary>Handles user login: verifies credentials and issues a JWT.</summary>
public class LoginHandler(IAppDbContext db, ITokenService tokenService, ILogger<LoginHandler> logger)
    : IRequestHandler<LoginCommand, AuthResultDto>
{
    /// <inheritdoc/>
    public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Email);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Password);

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        // Intentionally use the same log message for both failure paths to avoid
        // leaking user-existence information through log analysis.
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Failed login attempt for email {Email}.", request.Email);
            throw new UnauthorizedAccessException(Messages.Get("Auth_InvalidCredentials"));
        }

        // Resolve the primary household (owned first, then first membership)
        var household = await db.Households
            .FirstOrDefaultAsync(h => h.OwnerUserId == user.Id, cancellationToken);

        if (household is null)
        {
            var memberId = await db.HouseholdMembers
                .Where(m => m.UserId == user.Id)
                .Select(m => m.HouseholdId)
                .FirstOrDefaultAsync(cancellationToken);

            if (memberId != Guid.Empty)
                household = await db.Households.FindAsync([memberId], cancellationToken);
        }

        var householdId = household?.Id ?? Guid.Empty;
        logger.LogInformation("User {UserId} logged in from email {Email}.", user.Id, user.Email);
        var token = tokenService.GenerateToken(user, householdId);
        return new AuthResultDto(token, user.Id, user.Email, user.DisplayName, household?.Id);
    }
}
