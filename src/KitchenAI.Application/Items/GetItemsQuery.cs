using KitchenAI.Domain.Enums;
using MediatR;

namespace KitchenAI.Application.Items;

/// <summary>Queries inventory items for a household with optional filters.</summary>
public record GetItemsQuery(
    Guid HouseholdId,
    StorageLocation? Location = null,
    bool? ExpiringSoon = null,
    int ExpiryThresholdDays = 7) : IRequest<IList<ItemDto>>;
