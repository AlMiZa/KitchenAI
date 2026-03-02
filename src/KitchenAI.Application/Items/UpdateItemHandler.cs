using System.Text.Json;
using KitchenAI.Application.Persistence;
using KitchenAI.Application.Resources;
using KitchenAI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Items;

/// <summary>Handles update of an existing inventory item.</summary>
public class UpdateItemHandler(IAppDbContext db) : IRequestHandler<UpdateItemCommand, ItemDto>
{
    /// <inheritdoc/>
    public async Task<ItemDto> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);

        var item = await db.Items
            .FirstOrDefaultAsync(i => i.Id == request.ItemId && i.HouseholdId == request.HouseholdId, cancellationToken)
            ?? throw new KeyNotFoundException(Messages.Get("Item_NotFound", request.ItemId));

        var previousQuantity = item.Quantity;

        item.Name = request.Name;
        item.Quantity = request.Quantity;
        item.Unit = request.Unit;
        item.AllowFraction = request.AllowFraction;
        item.PurchaseDate = request.PurchaseDate;
        item.ExpiryDate = request.ExpiryDate;
        item.BestByOrUseBy = request.BestByOrUseBy;
        item.StorageLocation = request.StorageLocation;
        item.Brand = request.Brand;
        item.Price = request.Price;
        item.Notes = request.Notes;
        item.UpdatedAt = DateTime.UtcNow;

        // Record consumption event when quantity decreases
        if (request.Quantity < previousQuantity)
        {
            db.AnalyticsEvents.Add(new AnalyticsEvent
            {
                Id = Guid.NewGuid(),
                HouseholdId = request.HouseholdId,
                EventType = "item_consumed",
                Metadata = JsonSerializer.Serialize(new
                {
                    name = item.Name,
                    quantityConsumed = previousQuantity - request.Quantity,
                    price = item.Price
                }),
                CreatedAt = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync(cancellationToken);
        return CreateItemHandler.ToDto(item);
    }
}
