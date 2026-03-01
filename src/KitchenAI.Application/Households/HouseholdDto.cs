namespace KitchenAI.Application.Households;

/// <summary>Household data-transfer object.</summary>
public record HouseholdDto(
    Guid Id,
    string Name,
    Guid OwnerUserId,
    int MemberCount);
