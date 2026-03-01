using MediatR;

namespace KitchenAI.Application.Recipes;

/// <summary>Data needed to save a recipe to the household library.</summary>
public record RecipeDataDto(
    string Title,
    IList<RecipeIngredientDto> Ingredients,
    IList<string> Steps,
    string? Nutrition,
    int Servings,
    int PrepTime,
    int CookTime,
    string? Tags);

/// <summary>Saves a recipe (e.g. one chosen from generated suggestions) to the household library.</summary>
public record SaveRecipeCommand(Guid HouseholdId, RecipeDataDto RecipeData) : IRequest<RecipeDto>;
