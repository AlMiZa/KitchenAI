namespace KitchenAI.Application.Services;

/// <summary>Rate-limits recipe generation requests per household.</summary>
public interface IGenerationRateLimiter
{
    /// <summary>Returns true if the household is allowed to generate a recipe; false if quota exceeded.</summary>
    bool TryConsume(Guid householdId);
}
