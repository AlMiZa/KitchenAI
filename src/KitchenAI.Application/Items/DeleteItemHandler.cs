using System.Text.Json;
using KitchenAI.Application.Persistence;
using KitchenAI.Application.Resources;
using KitchenAI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Items;

/// <summary>Handles soft-deletion (archiving) of an inventory item.</summary>
public class DeleteItemHandler(IAppDbContext db) : IRequestHandler<DeleteItemCommand>
{
    /// <inheritdoc/>
    public async Task Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var item = await db.Items
            .FirstOrDefaultAsync(i => i.Id == request.ItemId && i.HouseholdId == request.HouseholdId, cancellationToken)
            ?? throw new KeyNotFoundException(Messages.Get("Item_NotFound", request.ItemId));

        item.IsArchived = true;
        item.UpdatedAt = DateTime.UtcNow;

        db.AnalyticsEvents.Add(new AnalyticsEvent
        {
            Id = Guid.NewGuid(),
            HouseholdId = request.HouseholdId,
            EventType = "item_removed",
            Metadata = JsonSerializer.Serialize(new { name = item.Name }),
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync(cancellationToken);
    }
}
