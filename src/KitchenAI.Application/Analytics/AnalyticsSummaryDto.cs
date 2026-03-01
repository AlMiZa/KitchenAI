namespace KitchenAI.Application.Analytics;

/// <summary>Analytics summary for a household.</summary>
public record AnalyticsSummaryDto(
    decimal MoneySavedEstimate,
    int ExpiredItemsCount,
    int RecipesGeneratedCount,
    IList<string> MostUsedIngredients);
