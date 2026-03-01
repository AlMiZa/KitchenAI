using KitchenAI.Domain.Enums;

namespace KitchenAI.Domain.Entities;

/// <summary>An inventory item belonging to a household.</summary>
public class Item
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }

    /// <summary>Metric unit: g, kg, ml, L, pcs.</summary>
    public string Unit { get; set; } = string.Empty;
    public bool AllowFraction { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public BestByOrUseBy? BestByOrUseBy { get; set; }
    public StorageLocation StorageLocation { get; set; }
    public string? Brand { get; set; }
    public decimal Price { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsArchived { get; set; }

    public Household Household { get; set; } = null!;
}
