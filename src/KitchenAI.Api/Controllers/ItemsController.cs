using KitchenAI.Application.Items;
using KitchenAI.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenAI.Api.Controllers;

/// <summary>Inventory item management for a household.</summary>
[Authorize]
[Route("api/households/{hid:guid}/items")]
public class ItemsController : ApiControllerBase
{
    /// <summary>Returns inventory items for the household.</summary>
    [HttpGet]
    public async Task<IActionResult> GetItems(
        Guid hid,
        [FromQuery] StorageLocation? location,
        [FromQuery] bool? expiringSoon,
        CancellationToken ct)
    {
        ValidateHouseholdAccess(hid);
        var result = await Mediator.Send(new GetItemsQuery(hid, location, expiringSoon), ct);
        return Ok(result);
    }

    /// <summary>Adds a new inventory item.</summary>
    [HttpPost]
    public async Task<IActionResult> CreateItem(Guid hid, [FromBody] CreateItemCommand command, CancellationToken ct)
    {
        ValidateHouseholdAccess(hid);
        var result = await Mediator.Send(command with { HouseholdId = hid }, ct);
        return CreatedAtAction(nameof(GetItems), result);
    }

    /// <summary>Updates an existing inventory item.</summary>
    [HttpPut("{itemId:guid}")]
    public async Task<IActionResult> UpdateItem(
        Guid hid,
        Guid itemId,
        [FromBody] UpdateItemCommand command,
        CancellationToken ct)
    {
        ValidateHouseholdAccess(hid);
        var result = await Mediator.Send(command with { HouseholdId = hid, ItemId = itemId }, ct);
        return Ok(result);
    }

    /// <summary>Soft-deletes (archives) an inventory item.</summary>
    [HttpDelete("{itemId:guid}")]
    public async Task<IActionResult> DeleteItem(Guid hid, Guid itemId, CancellationToken ct)
    {
        ValidateHouseholdAccess(hid);
        await Mediator.Send(new DeleteItemCommand(hid, itemId), ct);
        return NoContent();
    }

    /// <summary>Merges multiple items into one by summing their quantities.</summary>
    [HttpPost("merge")]
    public async Task<IActionResult> MergeItems(Guid hid, [FromBody] MergeItemsCommand command, CancellationToken ct)
    {
        ValidateHouseholdAccess(hid);
        var result = await Mediator.Send(command with { HouseholdId = hid }, ct);
        return Ok(result);
    }
}
