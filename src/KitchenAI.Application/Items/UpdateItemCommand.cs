using KitchenAI.Domain.Enums;
using MediatR;

namespace KitchenAI.Application.Items;

/// <summary>Updates an existing inventory item.</summary>
public record UpdateItemCommand(
    Guid HouseholdId,
    Guid ItemId,
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
