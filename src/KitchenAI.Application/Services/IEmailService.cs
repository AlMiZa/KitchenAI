namespace KitchenAI.Application.Services;

/// <summary>Sends transactional emails to users.</summary>
public interface IEmailService
{
    /// <summary>Sends a passwordless magic-link email to the specified address.</summary>
    Task SendMagicLinkAsync(string email, string token, CancellationToken cancellationToken = default);
}
