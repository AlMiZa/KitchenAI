namespace KitchenAI.Domain.Entities;

/// <summary>Represents a short-lived, signed token used for passwordless authentication.</summary>
public class MagicLinkToken
{
    public Guid Id { get; set; }

    /// <summary>The email address this token was issued for.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>The opaque token value sent to the user's email.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>UTC timestamp after which the token is no longer valid.</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>Whether the token has already been consumed.</summary>
    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }
}
