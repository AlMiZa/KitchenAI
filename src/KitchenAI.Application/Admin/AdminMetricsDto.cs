namespace KitchenAI.Application.Admin;

/// <summary>Admin metrics summary.</summary>
public record AdminMetricsDto(
    int TotalUsers,
    int TotalHouseholds,
    int TotalRecipesGenerated,
    int TotalItemsTracked);
