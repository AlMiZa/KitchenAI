using KitchenAI.Application.Recipes;
using KitchenAI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenAI.Api.Controllers;

/// <summary>Internal endpoints for adapters and direct LLM access (admin/service-to-service use).</summary>
[Authorize(Roles = "admin")]
[Route("internal")]
public class InternalController(IRecipeAdapter recipeAdapter, ILlmService llmService) : ApiControllerBase
{
    /// <summary>Searches for candidate recipes matching the given ingredient list.</summary>
    [HttpGet("adapters/recipes/search")]
    public async Task<IActionResult> SearchAdapterRecipes(
        [FromQuery] string ingredients,
        [FromQuery] string? diet,
        [FromQuery] int? maxTime,
        [FromQuery] int? servings,
        CancellationToken ct)
    {
        var ingredientList = (ingredients ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        var constraints = new RecipeConstraints(diet, null, maxTime, servings);
        var results = await recipeAdapter.SearchAsync(ingredientList, constraints, ct);
        return Ok(results);
    }

    /// <summary>Returns nutrition data for an externally sourced recipe.</summary>
    [HttpGet("adapters/recipes/{externalId}/nutrition")]
    public async Task<IActionResult> GetNutrition(string externalId, CancellationToken ct)
    {
        var nutrition = await recipeAdapter.GetNutritionAsync(externalId, ct);
        if (nutrition is null)
            return NotFound();
        return Ok(new { nutrition });
    }

    /// <summary>Calls the LLM directly with the provided ingredient list and constraints.</summary>
    [HttpPost("llm/generate-recipe")]
    public async Task<IActionResult> GenerateLlmRecipe(
        [FromBody] LlmGenerateRequest request,
        CancellationToken ct)
    {
        var constraints = request.Constraints ?? new RecipeConstraints();
        var recipes = await llmService.GenerateRecipesAsync(
            [],
            request.ExternalMatches ?? [],
            constraints,
            ct);
        return Ok(recipes);
    }
}

/// <summary>Request body for POST /internal/llm/generate-recipe.</summary>
/// <param name="IngredientList">Reserved for future use: structured ingredient list for prompt enrichment.</param>
/// <param name="ExternalMatches">Candidate recipes from an external adapter to pass to the LLM.</param>
/// <param name="Constraints">Dietary and time constraints for generation.</param>
/// <param name="Context">Reserved for future use: freeform context string for prompt conditioning.</param>
public record LlmGenerateRequest(
    IList<string>? IngredientList,
    IList<GeneratedRecipeDto>? ExternalMatches,
    RecipeConstraints? Constraints,
    string? Context);
