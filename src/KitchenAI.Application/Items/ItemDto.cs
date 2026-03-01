using KitchenAI.Domain.Enums;

namespace KitchenAI.Application.Items;

/// <summary>Data-transfer object mirroring an inventory item.</summary>
public record ItemDto(
    Guid Id,
    Guid HouseholdId,
    string Name,
    decimal Quantity,
    string Unit,
    bool AllowFraction,
    DateOnly? PurchaseDate,
    DateOnly? ExpiryDate,
    BestByOrUseBy? BestByOrUseBy,
    StorageLocation StorageLocation,
    string? Brand,
    decimal Price,
    string? Notes,
    bool IsArchived,
    DateTime CreatedAt,
    DateTime UpdatedAt);
