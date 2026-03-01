namespace KitchenAI.Application.Recipes;

/// <summary>Normalised ingredient as part of a recipe.</summary>
public record RecipeIngredientDto(
    Guid Id,
    string Name,
    decimal Quantity,
    string Unit);
