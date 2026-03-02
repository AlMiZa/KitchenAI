using System.Text.Json;
using KitchenAI.Application.Exceptions;
using KitchenAI.Application.Persistence;
using KitchenAI.Application.Resources;
using KitchenAI.Application.Services;
using KitchenAI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace KitchenAI.Application.Recipes;

/// <summary>Handles recipe generation via the hybrid adapter + LLM flow.</summary>
public class GenerateRecipeHandler(
    IAppDbContext db,
    ILlmService llmService,
    IRecipeAdapter recipeAdapter,
    IGenerationRateLimiter rateLimiter,
    IMemoryCache memoryCache)
    : IRequestHandler<GenerateRecipeCommand, IList<GeneratedRecipeDto>>
{
    /// <inheritdoc/>
    public async Task<IList<GeneratedRecipeDto>> Handle(
        GenerateRecipeCommand request,
        CancellationToken cancellationToken)
    {
        // Rate-limit per household
        if (!rateLimiter.TryConsume(request.HouseholdId))
            throw new RateLimitExceededException(Messages.Get("Recipe_RateLimitExceeded", request.HouseholdId));

        // Load non-archived, non-zero-quantity items
        var items = await db.Items
            .Where(i => i.HouseholdId == request.HouseholdId && !i.IsArchived && i.Quantity > 0)
            .ToListAsync(cancellationToken);

        var constraints = request.Constraints ?? new RecipeConstraints();

        // Cache key from sorted item IDs + constraints snapshot
        var inventoryKey = ComputeCacheKey(request.HouseholdId, items, constraints);
        if (memoryCache.TryGetValue(inventoryKey, out IList<GeneratedRecipeDto>? cached) && cached is not null)
            return cached;

        // Step 1: Query recipe adapter for candidate recipes
        var ingredientNames = items
            .OrderByDescending(i => i.Quantity)
            .Select(i => i.Name)
            .Distinct()
            .ToList();

        var candidateRecipes = await recipeAdapter.SearchAsync(ingredientNames, constraints, cancellationToken);

        // Step 2: Call LLM with items, candidates, and constraints
        var llmRecipes = await llmService.GenerateRecipesAsync(items, candidateRecipes, constraints, cancellationToken);

        // Build prompt template after LLM call (audit only; not passed to LLM in stub)
        var promptTemplate = BuildPromptTemplate(items, candidateRecipes, constraints);

        // Step 3: Validate LLM output; fall back to adapter candidates if invalid
        var recipes = IsValidLlmOutput(llmRecipes)
            ? llmRecipes
            : candidateRecipes.Count >= 2
                ? candidateRecipes
                : throw new InvalidOperationException(Messages.Get("Recipe_LlmOutputInvalid"));

        // Ensure at least 2 distinct recipes
        if (recipes.Count < 2)
            throw new InvalidOperationException(Messages.Get("Recipe_TooFewGenerated"));

        var llmResponseJson = JsonSerializer.Serialize(llmRecipes);

        var generatedRecord = new GeneratedRecipe
        {
            Id = Guid.NewGuid(),
            HouseholdId = request.HouseholdId,
            RecipeJson = JsonSerializer.Serialize(recipes),
            LlmResponseJson = llmResponseJson,
            PromptTemplate = promptTemplate,
            Rationale = "Generated from household inventory",
            RequestedBy = request.RequestedByUserId,
            MatchedInventorySnapshot = JsonSerializer.Serialize(items.Select(i => i.Id)),
            CreatedAt = DateTime.UtcNow
        };

        db.GeneratedRecipes.Add(generatedRecord);
        db.AnalyticsEvents.Add(new AnalyticsEvent
        {
            Id = Guid.NewGuid(),
            HouseholdId = request.HouseholdId,
            EventType = "recipe_generated",
            Metadata = JsonSerializer.Serialize(new { recipeCount = recipes.Count }),
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync(cancellationToken);

        // Cache the result for identical inventory snapshots (10 minutes)
        memoryCache.Set(inventoryKey, recipes, TimeSpan.FromMinutes(10));

        return recipes;
    }

    /// <summary>Validates that LLM output has the required structure per recipe.</summary>
    private static bool IsValidLlmOutput(IList<GeneratedRecipeDto> recipes) =>
        recipes.Count >= 2 &&
        recipes.All(r =>
            !string.IsNullOrWhiteSpace(r.Title) &&
            r.Ingredients.Count > 0 &&
            r.Steps.Count > 0);

    /// <summary>Builds a structured prompt template string for audit traceability.</summary>
    private static string BuildPromptTemplate(IList<Item> items, IList<GeneratedRecipeDto> candidates, RecipeConstraints constraints)
    {
        var ingredientList = string.Join(", ", items.Select(i => $"{i.Name} ({i.Quantity} {i.Unit})"));
        var candidateTitles = string.Join(", ", candidates.Select(c => c.Title));
        return $"Generate 2 distinct recipes using: [{ingredientList}]. " +
               $"Candidate recipes from DB: [{candidateTitles}]. " +
               $"Constraints: diet={constraints.Diet}, maxTime={constraints.MaxTime}, servings={constraints.Servings}. " +
               $"MaxTokens=2048.";
    }

    private static string ComputeCacheKey(Guid householdId, IList<Item> items, RecipeConstraints constraints)
    {
        // Include item IDs and quantities so cache is invalidated when quantities change
        var sortedItems = string.Join(",", items.OrderBy(i => i.Id).Select(i => $"{i.Id}:{i.Quantity}"));
        return $"gen_cache:{householdId}:{sortedItems}:{constraints.Diet}:{constraints.MaxTime}:{constraints.Servings}";
    }
}
