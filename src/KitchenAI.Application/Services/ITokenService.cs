using KitchenAI.Domain.Entities;

namespace KitchenAI.Application.Services;

/// <summary>Generates signed JWT tokens for authenticated users.</summary>
public interface ITokenService
{
    /// <summary>Creates a signed JWT for the given user scoped to the specified household.</summary>
    string GenerateToken(User user, Guid householdId);
}
