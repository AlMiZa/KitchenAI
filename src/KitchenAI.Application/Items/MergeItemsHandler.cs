using KitchenAI.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Items;

/// <summary>Merges a list of items into the first one by summing quantities; archives the rest.</summary>
public class MergeItemsHandler(IAppDbContext db) : IRequestHandler<MergeItemsCommand, ItemDto>
{
    /// <inheritdoc/>
    public async Task<ItemDto> Handle(MergeItemsCommand request, CancellationToken cancellationToken)
    {
        if (request.ItemIds.Count < 2)
            throw new ArgumentException("At least two items are required to merge.");

        var items = await db.Items
            .Where(i => request.ItemIds.Contains(i.Id) && i.HouseholdId == request.HouseholdId && !i.IsArchived)
            .OrderBy(i => i.CreatedAt)
            .ToListAsync(cancellationToken);

        if (items.Count < 2)
            throw new KeyNotFoundException("Could not find at least two matching items to merge.");

        var primary = items[0];
        primary.Quantity = items.Sum(i => i.Quantity);
        primary.UpdatedAt = DateTime.UtcNow;

        foreach (var other in items.Skip(1))
        {
            other.IsArchived = true;
            other.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(cancellationToken);
        return CreateItemHandler.ToDto(primary);
    }
}
