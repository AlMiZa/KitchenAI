using KitchenAI.Domain.Enums;

namespace KitchenAI.Application.Recipes;

/// <summary>Saved recipe with its normalised ingredients.</summary>
public record RecipeDto(
    Guid Id,
    Guid HouseholdId,
    string Title,
    RecipeSource Source,
    IList<RecipeIngredientDto> Ingredients,
    IList<string> Steps,
    NutritionDto? Nutrition,
    int Servings,
    int PrepTime,
    int CookTime,
    string? Tags,
    DateTime CreatedAt);
