namespace KitchenAI.Application.Recipes;

/// <summary>LLM-generated recipe suggestion (not yet saved to the library).</summary>
public record GeneratedRecipeDto(
    Guid Id,
    string Title,
    IList<RecipeIngredientDto> Ingredients,
    IList<string> Steps,
    NutritionDto? Nutrition,
    string? Rationale,
    int PrepTime,
    int CookTime,
    int Servings);
