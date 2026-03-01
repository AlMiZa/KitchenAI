using KitchenAI.Domain.Enums;

namespace KitchenAI.Domain.Entities;

/// <summary>A saved recipe belonging to a household.</summary>
public class Recipe
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public RecipeSource Source { get; set; }
    public string Title { get; set; } = string.Empty;

    /// <summary>JSON-serialised ingredient list (or use normalised RecipeIngredients).</summary>
    public string? Ingredients { get; set; }

    /// <summary>JSON-serialised preparation steps.</summary>
    public string? Steps { get; set; }

    /// <summary>JSON-serialised nutrition information.</summary>
    public string? Nutrition { get; set; }
    public int Servings { get; set; }
    public int PrepTime { get; set; }
    public int CookTime { get; set; }

    /// <summary>Comma-separated tags (cuisine, diet labels).</summary>
    public string? Tags { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    /// <summary>JSON-serialised LLM response and substitution rationale.</summary>
    public string? GeneratedBy { get; set; }

    public Household Household { get; set; } = null!;
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];
}
