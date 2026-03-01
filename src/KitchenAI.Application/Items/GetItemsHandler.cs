using KitchenAI.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Application.Items;

/// <summary>Returns inventory items for a household with optional filtering.</summary>
public class GetItemsHandler(IAppDbContext db) : IRequestHandler<GetItemsQuery, IList<ItemDto>>
{
    /// <inheritdoc/>
    public async Task<IList<ItemDto>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
    {
        var query = db.Items
            .Where(i => i.HouseholdId == request.HouseholdId && !i.IsArchived);

        if (request.Location.HasValue)
            query = query.Where(i => i.StorageLocation == request.Location.Value);

        if (request.ExpiringSoon == true)
        {
            var threshold = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(request.ExpiryThresholdDays));
            query = query.Where(i => i.ExpiryDate.HasValue && i.ExpiryDate.Value <= threshold);
        }

        var items = await query
            .OrderBy(i => i.ExpiryDate)
            .ToListAsync(cancellationToken);

        return items.Select(CreateItemHandler.ToDto).ToList();
    }
}
