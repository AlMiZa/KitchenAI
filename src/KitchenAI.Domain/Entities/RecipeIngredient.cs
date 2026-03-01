namespace KitchenAI.Domain.Entities;

/// <summary>Normalised ingredient row belonging to a recipe.</summary>
public class RecipeIngredient
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;

    public Recipe Recipe { get; set; } = null!;
}
