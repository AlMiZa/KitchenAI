using MediatR;

namespace KitchenAI.Application.Items;

/// <summary>Soft-deletes (archives) an inventory item.</summary>
public record DeleteItemCommand(Guid HouseholdId, Guid ItemId) : IRequest;
