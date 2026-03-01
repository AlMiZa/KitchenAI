namespace KitchenAI.Application.Recipes;

/// <summary>Dietary and time constraints for recipe generation.</summary>
public record RecipeConstraints(
    string? Diet = null,
    IList<string>? Allergies = null,
    int? MaxTime = null,
    int? Servings = null);
