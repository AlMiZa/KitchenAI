using MediatR;

namespace KitchenAI.Application.Items;

/// <summary>Merges multiple items into one by summing their quantities.</summary>
public record MergeItemsCommand(Guid HouseholdId, IList<Guid> ItemIds) : IRequest<ItemDto>;
