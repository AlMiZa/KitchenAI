using KitchenAI.Application.Recipes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenAI.Api.Controllers;

/// <summary>Recipe library management and AI generation for a household.</summary>
[Authorize]
[Route("api/households/{hid:guid}/recipes")]
public class RecipesController : ApiControllerBase
{
    /// <summary>Returns all saved recipes for the household.</summary>
    [HttpGet]
    public async Task<IActionResult> GetRecipes(Guid hid, CancellationToken ct)
    {
        ValidateHouseholdAccess(hid);
        var result = await Mediator.Send(new GetRecipesQuery(hid), ct);
        return Ok(result);
    }

    /// <summary>Saves a recipe to the household library.</summary>
    [HttpPost]
    public async Task<IActionResult> SaveRecipe(Guid hid, [FromBody] RecipeDataDto recipeData, CancellationToken ct)
    {
        ValidateHouseholdAccess(hid);
        var result = await Mediator.Send(new SaveRecipeCommand(hid, recipeData), ct);
        return CreatedAtAction(nameof(GetRecipes), new { hid }, result);
    }

    /// <summary>Returns a single saved recipe.</summary>
    [HttpGet("{rid:guid}")]
    public async Task<IActionResult> GetRecipe(Guid hid, Guid rid, CancellationToken ct)
    {
        ValidateHouseholdAccess(hid);
        var result = await Mediator.Send(new GetRecipeQuery(hid, rid), ct);
        return Ok(result);
    }

    /// <summary>Generates recipe suggestions based on the household inventory.</summary>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateRecipes(
        Guid hid,
        [FromBody] RecipeConstraints? constraints,
        CancellationToken ct)
    {
        ValidateHouseholdAccess(hid);
        var result = await Mediator.Send(
            new GenerateRecipeCommand(hid, GetCurrentUserId(), constraints), ct);
        return Ok(result);
    }

    /// <summary>Checks whether all ingredients for a recipe are available in the household inventory.</summary>
    [HttpPost("{rid:guid}/check")]
    public async Task<IActionResult> CheckAvailability(Guid hid, Guid rid, CancellationToken ct)
    {
        ValidateHouseholdAccess(hid);
        var result = await Mediator.Send(new CheckRecipeAvailabilityQuery(hid, rid), ct);
        return Ok(result);
    }

    /// <summary>Records that the household cooked a recipe.</summary>
    [HttpPost("{rid:guid}/cook")]
    public async Task<IActionResult> CookRecipe(Guid hid, Guid rid, CancellationToken ct)
    {
        ValidateHouseholdAccess(hid);
        await Mediator.Send(new CookRecipeCommand(hid, rid), ct);
        return NoContent();
    }
}
