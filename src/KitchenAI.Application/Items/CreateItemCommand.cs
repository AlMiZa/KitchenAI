using KitchenAI.Domain.Enums;
using MediatR;

namespace KitchenAI.Application.Items;

/// <summary>Creates a new inventory item in the specified household.</summary>
public record CreateItemCommand(
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
    string? Notes) : IRequest<ItemDto>;
