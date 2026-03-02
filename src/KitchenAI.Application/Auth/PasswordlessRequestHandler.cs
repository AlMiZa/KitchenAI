using System.Security.Cryptography;
using KitchenAI.Application.Persistence;
using KitchenAI.Application.Services;
using KitchenAI.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KitchenAI.Application.Auth;

/// <summary>Generates a short-lived magic-link token, persists it, and emails it to the user.</summary>
public class PasswordlessRequestHandler(IAppDbContext db, IEmailService emailService, ILogger<PasswordlessRequestHandler> logger)
    : IRequestHandler<PasswordlessRequestCommand>
{
    /// <inheritdoc/>
    public async Task Handle(PasswordlessRequestCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Email);

        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        var magicLink = new MagicLinkToken
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Token = rawToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        db.MagicLinkTokens.Add(magicLink);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Magic link token created for email {Email}.", request.Email);

        await emailService.SendMagicLinkAsync(request.Email, rawToken, cancellationToken);
    }
}
