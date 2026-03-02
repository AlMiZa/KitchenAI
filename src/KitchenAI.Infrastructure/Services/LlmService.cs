using KitchenAI.Application.Recipes;
using KitchenAI.Application.Services;
using KitchenAI.Domain.Entities;

namespace KitchenAI.Infrastructure.Services;

/// <summary>
/// Stub LLM service that generates plausible recipes from the household inventory without
/// calling an external API. In production, replace this with a Gemini API call.
/// </summary>
public class LlmService : ILlmService
{
    /// <inheritdoc/>
    public Task<List<GeneratedRecipeDto>> GenerateRecipesAsync(
        IList<Item> items,
        IList<GeneratedRecipeDto> candidateRecipes,
        RecipeConstraints constraints,
        CancellationToken cancellationToken = default)
    {
        var itemNames = items
            .OrderByDescending(i => i.Quantity)
            .Select(i => i.Name)
            .Distinct()
            .Take(5)
            .ToList();

        var primary = itemNames.ElementAtOrDefault(0) ?? "Vegetables";
        var secondary = itemNames.ElementAtOrDefault(1) ?? "Herbs";
        var tertiary = itemNames.ElementAtOrDefault(2) ?? "Spice";

        var servings = constraints.Servings ?? 4;
        var maxTime = constraints.MaxTime ?? 30;

        var recipe1 = new GeneratedRecipeDto(
            Id: Guid.NewGuid(),
            Title: $"{primary} & {secondary} Stir-Fry",
            Ingredients:
            [
                new RecipeIngredientDto(Guid.NewGuid(), primary, 200, "g"),
                new RecipeIngredientDto(Guid.NewGuid(), secondary, 50, "g"),
                new RecipeIngredientDto(Guid.NewGuid(), "Olive oil", 2, "tbsp"),
                new RecipeIngredientDto(Guid.NewGuid(), "Salt", 1, "tsp")
            ],
            Steps:
            [
                $"Prepare and chop {primary} into bite-sized pieces.",
                $"Heat olive oil in a pan over medium-high heat.",
                $"Add {primary} and stir-fry for 5 minutes.",
                $"Add {secondary} and season with salt. Cook for another 3 minutes.",
                "Serve immediately."
            ],
            Nutrition: new NutritionDto(300 + (int)(items.Count * 5), 15, 35, 10),
            Rationale: $"Uses your {primary} and {secondary} which are available in your pantry.",
            PrepTime: 10,
            CookTime: maxTime / 2,
            Servings: servings);

        var recipe2 = new GeneratedRecipeDto(
            Id: Guid.NewGuid(),
            Title: $"Simple {primary} Soup",
            Ingredients:
            [
                new RecipeIngredientDto(Guid.NewGuid(), primary, 300, "g"),
                new RecipeIngredientDto(Guid.NewGuid(), tertiary, 5, "g"),
                new RecipeIngredientDto(Guid.NewGuid(), "Water", 500, "ml"),
                new RecipeIngredientDto(Guid.NewGuid(), "Salt", 1, "tsp")
            ],
            Steps:
            [
                $"Dice {primary} into small cubes.",
                "Bring water to a boil in a pot.",
                $"Add {primary} and {tertiary}, reduce heat to medium.",
                "Simmer for 20 minutes, season with salt.",
                "Blend if desired and serve hot."
            ],
            Nutrition: new NutritionDto(200 + (int)(items.Count * 3), 8, 28, 5),
            Rationale: $"A warming soup made primarily from your {primary} stock.",
            PrepTime: 5,
            CookTime: maxTime,
            Servings: servings);

        return Task.FromResult(new List<GeneratedRecipeDto> { recipe1, recipe2 });
    }
}
