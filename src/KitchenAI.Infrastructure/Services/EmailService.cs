using KitchenAI.Application.Services;
using Microsoft.Extensions.Logging;

namespace KitchenAI.Infrastructure.Services;

/// <summary>
/// Log-only email service used as the default implementation.
/// Replace with an SMTP or third-party provider in production.
/// </summary>
public class EmailService(ILogger<EmailService> logger) : IEmailService
{
    /// <inheritdoc/>
    public Task SendMagicLinkAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        // In production, send a real email with a link such as:
        // https://app.kitchenai.com/auth/magic?token={token}
        logger.LogInformation("Magic-link token for {Email}: {Token}", email, token);
        return Task.CompletedTask;
    }
}
