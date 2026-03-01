using System.Text.Json;
using KitchenAI.Application.Exceptions;
using KitchenAI.Application.Persistence;
using KitchenAI.Application.Services;
using KitchenAI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Auth;

/// <summary>Handles user registration: creates User, Household, HouseholdMember and issues a JWT.</summary>
public class RegisterHandler(IAppDbContext db, ITokenService tokenService)
    : IRequestHandler<RegisterCommand, AuthResultDto>
{
    /// <inheritdoc/>
    public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Email);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Password);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.DisplayName);

        if (await db.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
            throw new ValidationException("Email already in use");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            DisplayName = request.DisplayName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var household = new Household
        {
            Id = Guid.NewGuid(),
            Name = $"{request.DisplayName}'s Household",
            OwnerUserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        var member = new HouseholdMember
        {
            Id = Guid.NewGuid(),
            HouseholdId = household.Id,
            UserId = user.Id,
            Role = "owner"
        };

        var analyticsEvent = new AnalyticsEvent
        {
            Id = Guid.NewGuid(),
            HouseholdId = household.Id,
            EventType = "user_registered",
            Metadata = JsonSerializer.Serialize(new { email = request.Email }),
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        db.Households.Add(household);
        db.HouseholdMembers.Add(member);
        db.AnalyticsEvents.Add(analyticsEvent);
        await db.SaveChangesAsync(cancellationToken);

        var token = tokenService.GenerateToken(user, household.Id);
        return new AuthResultDto(token, user.Id, user.Email, user.DisplayName, household.Id);
    }
}
