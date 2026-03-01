using KitchenAI.Application.Gdpr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenAI.Api.Controllers;

/// <summary>GDPR data export and deletion endpoints.</summary>
[Authorize]
[Route("api/users")]
public class UsersController : ApiControllerBase
{
    /// <summary>Exports all of the current user's data as a JSON file download.</summary>
    [HttpGet("me/export")]
    public async Task<IActionResult> ExportData(CancellationToken ct)
    {
        var json = await Mediator.Send(new ExportUserDataQuery(GetCurrentUserId()), ct);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        return File(bytes, "application/json", "my-data.json");
    }

    /// <summary>Permanently deletes the current user and all associated data.</summary>
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteAccount(CancellationToken ct)
    {
        await Mediator.Send(new DeleteUserCommand(GetCurrentUserId()), ct);
        return NoContent();
    }
}
