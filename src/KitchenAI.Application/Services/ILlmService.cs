using KitchenAI.Application.Recipes;
using KitchenAI.Domain.Entities;

namespace KitchenAI.Application.Services;

/// <summary>Generates recipe suggestions, optionally using an LLM backend.</summary>
public interface ILlmService
{
    /// <summary>Returns a list of recipe suggestions based on the household's inventory and constraints.</summary>
    Task<List<GeneratedRecipeDto>> GenerateRecipesAsync(
        IList<Item> items,
        RecipeConstraints constraints,
        CancellationToken cancellationToken = default);
}
