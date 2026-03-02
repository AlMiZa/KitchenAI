namespace KitchenAI.Application.Recipes;

/// <summary>Structured nutritional information per serving.</summary>
public record NutritionDto(double? Calories, double? Protein, double? Carbs, double? Fat);
