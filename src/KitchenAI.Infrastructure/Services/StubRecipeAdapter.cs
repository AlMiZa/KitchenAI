using KitchenAI.Application.Recipes;
using KitchenAI.Application.Services;

namespace KitchenAI.Infrastructure.Services;

/// <summary>
/// Stub recipe adapter that returns plausible fallback recipes without calling an external API.
/// In production, replace with a real recipe database adapter (e.g., Spoonacular).
/// </summary>
public class StubRecipeAdapter : IRecipeAdapter
{
    /// <inheritdoc/>
    public Task<IList<GeneratedRecipeDto>> SearchAsync(
        IList<string> ingredients,
        RecipeConstraints constraints,
        CancellationToken cancellationToken = default)
    {
        var primary = ingredients.ElementAtOrDefault(0) ?? "Vegetables";
        var secondary = ingredients.ElementAtOrDefault(1) ?? "Herbs";
        var servings = constraints.Servings ?? 4;

        IList<GeneratedRecipeDto> results =
        [
            new GeneratedRecipeDto(
                Id: Guid.NewGuid(),
                Title: $"Adapter: {primary} Bowl",
                Ingredients: [new RecipeIngredientDto(Guid.NewGuid(), primary, 200, "g")],
                Steps: [$"Prepare {primary}.", "Serve in a bowl."],
                Nutrition: "~250 kcal per serving",
                Rationale: $"A simple bowl featuring {primary} from your pantry.",
                PrepTime: 5,
                CookTime: 15,
                Servings: servings),
            new GeneratedRecipeDto(
                Id: Guid.NewGuid(),
                Title: $"Adapter: {primary} & {secondary} Mix",
                Ingredients:
                [
                    new RecipeIngredientDto(Guid.NewGuid(), primary, 150, "g"),
                    new RecipeIngredientDto(Guid.NewGuid(), secondary, 50, "g")
                ],
                Steps: [$"Mix {primary} and {secondary}.", "Season and serve."],
                Nutrition: "~200 kcal per serving",
                Rationale: $"Combines {primary} and {secondary} available in your inventory.",
                PrepTime: 5,
                CookTime: 10,
                Servings: servings)
        ];

        return Task.FromResult(results);
    }

    /// <inheritdoc/>
    public Task<string?> GetNutritionAsync(string externalId, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>("~200 kcal per serving (estimated)");
}
