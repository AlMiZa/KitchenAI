using KitchenAI.Application.Recipes;
using KitchenAI.Application.Services;
using KitchenAI.Domain.Entities;

namespace KitchenAI.Infrastructure.Services;

/// <summary>
/// Stub LLM service that generates plausible recipes from the household inventory without
/// calling an external API. Picks 2 random templates per call for variety.
/// In production, replace this with a Gemini API call.
/// </summary>
public class LlmService : ILlmService
{
    private static readonly Random _rng = Random.Shared;

    /// <inheritdoc/>
    public Task<List<GeneratedRecipeDto>> GenerateRecipesAsync(
        IList<Item> items,
        IList<GeneratedRecipeDto> candidateRecipes,
        RecipeConstraints constraints,
        CancellationToken cancellationToken = default)
    {
        // Shuffle item names so each call can pick a different "primary" ingredient
        var itemNames = items
            .OrderBy(_ => _rng.Next())
            .Select(i => i.Name)
            .Distinct()
            .Take(5)
            .ToList();

        var primary   = itemNames.ElementAtOrDefault(0) ?? "Vegetables";
        var secondary = itemNames.ElementAtOrDefault(1) ?? "Herbs";
        var tertiary  = itemNames.ElementAtOrDefault(2) ?? "Spice";
        var quaternary = itemNames.ElementAtOrDefault(3) ?? "Garlic";

        var servings = constraints.Servings ?? 4;
        var maxTime  = constraints.MaxTime  ?? 30;
        var baseKcal = 200 + items.Count * 4;

        // Pool of 6 recipe templates — 2 are chosen at random per call
        var pool = new List<GeneratedRecipeDto>
        {
            new(Guid.NewGuid(),
                $"{primary} & {secondary} Stir-Fry",
                [
                    new(Guid.NewGuid(), primary,     200, "g"),
                    new(Guid.NewGuid(), secondary,    50, "g"),
                    new(Guid.NewGuid(), "Olive oil",   2, "tbsp"),
                    new(Guid.NewGuid(), "Salt",         1, "tsp"),
                ],
                [$"Chop {primary} into bite-sized pieces.", "Heat oil in a pan.", $"Stir-fry {primary} for 5 min.", $"Add {secondary}, season and cook 3 more min.", "Serve immediately."],
                new NutritionDto(baseKcal + 100, 15, 35, 10),
                $"Uses your {primary} and {secondary} available in the pantry.",
                10, maxTime / 2, servings),

            new(Guid.NewGuid(),
                $"Simple {primary} Soup",
                [
                    new(Guid.NewGuid(), primary,   300, "g"),
                    new(Guid.NewGuid(), tertiary,    5, "g"),
                    new(Guid.NewGuid(), "Water",   500, "ml"),
                    new(Guid.NewGuid(), "Salt",      1, "tsp"),
                ],
                [$"Dice {primary}.", "Boil water in a pot.", $"Add {primary} and {tertiary}, simmer 20 min.", "Season with salt.", "Blend if desired and serve hot."],
                new NutritionDto(baseKcal, 8, 28, 5),
                $"A warming soup from your {primary} stock.",
                5, maxTime, servings),

            new(Guid.NewGuid(),
                $"{primary} & {tertiary} Bake",
                [
                    new(Guid.NewGuid(), primary,    250, "g"),
                    new(Guid.NewGuid(), tertiary,    10, "g"),
                    new(Guid.NewGuid(), "Olive oil",  3, "tbsp"),
                    new(Guid.NewGuid(), "Pepper",     1, "tsp"),
                ],
                [$"Preheat oven to 200°C.", $"Toss {primary} with oil and {tertiary}.", "Spread on a baking tray.", "Bake 25 min until golden.", "Serve warm."],
                new NutritionDto(baseKcal + 50, 12, 30, 12),
                $"Roasted {primary} with {tertiary} — minimal prep, great flavour.",
                10, 30, servings),

            new(Guid.NewGuid(),
                $"{secondary} & {quaternary} Pasta",
                [
                    new(Guid.NewGuid(), "Pasta",      200, "g"),
                    new(Guid.NewGuid(), secondary,     80, "g"),
                    new(Guid.NewGuid(), quaternary,    20, "g"),
                    new(Guid.NewGuid(), "Olive oil",    2, "tbsp"),
                    new(Guid.NewGuid(), "Salt",          1, "tsp"),
                ],
                ["Cook pasta al dente and drain.", $"Sauté {quaternary} in oil 2 min.", $"Add {secondary}, cook 4 min.", "Toss with pasta.", "Season and serve."],
                new NutritionDto(baseKcal + 150, 18, 55, 8),
                $"Quick pasta using {secondary} and {quaternary} from your kitchen.",
                10, 20, servings),

            new(Guid.NewGuid(),
                $"{primary} Omelette",
                [
                    new(Guid.NewGuid(), "Eggs",       3, "pcs"),
                    new(Guid.NewGuid(), primary,      80, "g"),
                    new(Guid.NewGuid(), secondary,    30, "g"),
                    new(Guid.NewGuid(), "Butter",      1, "tbsp"),
                    new(Guid.NewGuid(), "Salt",         1, "tsp"),
                ],
                ["Beat eggs with salt.", $"Sauté {primary} and {secondary} in butter 3 min.", "Pour eggs over, cook on low until set.", "Fold and serve."],
                new NutritionDto(baseKcal - 20, 20, 10, 15),
                $"A protein-rich omelette using {primary} already in your fridge.",
                5, 10, servings),

            new(Guid.NewGuid(),
                $"{primary} & {secondary} Salad",
                [
                    new(Guid.NewGuid(), primary,   150, "g"),
                    new(Guid.NewGuid(), secondary,  80, "g"),
                    new(Guid.NewGuid(), tertiary,   10, "g"),
                    new(Guid.NewGuid(), "Olive oil", 2, "tbsp"),
                    new(Guid.NewGuid(), "Lemon juice", 1, "tbsp"),
                ],
                [$"Slice {primary} and {secondary}.", "Combine in a bowl.", $"Add {tertiary} for texture.", "Drizzle oil and lemon juice.", "Toss and serve fresh."],
                new NutritionDto(baseKcal - 60, 6, 20, 9),
                $"Light and fresh — uses {primary} and {secondary} before they expire.",
                5, 0, servings),
        };

        // Pick 2 distinct templates at random
        var picked = pool.OrderBy(_ => _rng.Next()).Take(2).ToList();
        return Task.FromResult(picked);
    }
}
