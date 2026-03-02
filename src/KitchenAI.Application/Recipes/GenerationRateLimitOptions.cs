namespace KitchenAI.Application.Recipes;

/// <summary>Configuration options for recipe generation rate limiting.</summary>
public class GenerationRateLimitOptions
{
    public const string SectionName = "RecipeGeneration";

    /// <summary>Maximum number of recipe generation requests per household per hour. Default: 10.</summary>
    public int MaxRequestsPerHour { get; set; } = 10;
}
