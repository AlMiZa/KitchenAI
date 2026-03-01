using System.Text.Json;
using KitchenAI.Application.Exceptions;
using KitchenAI.Application.Persistence;
using KitchenAI.Domain.Entities;
using MediatR;

namespace KitchenAI.Application.Items;

/// <summary>Handles creation of a new inventory item.</summary>
public class CreateItemHandler(IAppDbContext db) : IRequestHandler<CreateItemCommand, ItemDto>
{
    /// <inheritdoc/>
    public async Task<ItemDto> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);

        var item = new Item
        {
            Id = Guid.NewGuid(),
            HouseholdId = request.HouseholdId,
            Name = request.Name,
            Quantity = request.Quantity,
            Unit = request.Unit,
            AllowFraction = request.AllowFraction,
            PurchaseDate = request.PurchaseDate,
            ExpiryDate = request.ExpiryDate,
            BestByOrUseBy = request.BestByOrUseBy,
            StorageLocation = request.StorageLocation,
            Brand = request.Brand,
            Price = request.Price,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var evt = new AnalyticsEvent
        {
            Id = Guid.NewGuid(),
            HouseholdId = request.HouseholdId,
            EventType = "item_added",
            Metadata = JsonSerializer.Serialize(new
            {
                name = request.Name,
                quantity = request.Quantity,
                unit = request.Unit,
                price = request.Price
            }),
            CreatedAt = DateTime.UtcNow
        };

        db.Items.Add(item);
        db.AnalyticsEvents.Add(evt);
        await db.SaveChangesAsync(cancellationToken);

        return ToDto(item);
    }

    internal static ItemDto ToDto(Item item) => new(
        item.Id, item.HouseholdId, item.Name, item.Quantity, item.Unit,
        item.AllowFraction, item.PurchaseDate, item.ExpiryDate, item.BestByOrUseBy,
        item.StorageLocation, item.Brand, item.Price, item.Notes,
        item.IsArchived, item.CreatedAt, item.UpdatedAt);
}
