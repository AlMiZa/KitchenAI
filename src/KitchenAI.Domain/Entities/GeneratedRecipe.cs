namespace KitchenAI.Domain.Entities;

/// <summary>Audit record of a full LLM recipe-generation request and its result.</summary>
public class GeneratedRecipe
{
    public Guid Id { get; set; }

    /// <summary>Full JSON response returned by the LLM.</summary>
    public string RecipeJson { get; set; } = string.Empty;
    public string? Rationale { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid RequestedBy { get; set; }

    /// <summary>JSON snapshot of the inventory item IDs used during generation.</summary>
    public string? MatchedInventorySnapshot { get; set; }

    public User RequestedByUser { get; set; } = null!;
}
