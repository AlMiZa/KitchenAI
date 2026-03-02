using KitchenAI.Application.Recipes;

namespace KitchenAI.Application.Services;

/// <summary>Adapter for querying an external recipe database.</summary>
public interface IRecipeAdapter
{
    /// <summary>Searches for candidate recipes matching the given ingredients and constraints.</summary>
    Task<IList<GeneratedRecipeDto>> SearchAsync(
        IList<string> ingredients,
        RecipeConstraints constraints,
        CancellationToken cancellationToken = default);

    /// <summary>Returns nutrition info for an externally sourced recipe, or null if unavailable.</summary>
    Task<string?> GetNutritionAsync(string externalId, CancellationToken cancellationToken = default);
}
