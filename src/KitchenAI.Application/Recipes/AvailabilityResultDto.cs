namespace KitchenAI.Application.Recipes;

/// <summary>Overall result of a recipe availability check.</summary>
public record AvailabilityResultDto(string Status, IList<AvailabilityItemDto>? Items);

/// <summary>Per-ingredient availability detail.</summary>
public record AvailabilityItemDto(
    string Name,
    decimal Required,
    decimal Available,
    decimal Deficit);
