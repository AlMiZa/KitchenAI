using KitchenAI.Application.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenAI.Api.Controllers;

/// <summary>Admin-only configuration endpoints.</summary>
[Authorize(Roles = "admin")]
[Route("api/admin")]
public class AdminController : ApiControllerBase
{
    /// <summary>Returns the current admin configuration status.</summary>
    [HttpGet("config")]
    public async Task<IActionResult> GetConfig(CancellationToken ct)
    {
        var result = await Mediator.Send(new GetAdminConfigQuery(), ct);
        return Ok(result);
    }

    /// <summary>Updates mutable admin configuration.</summary>
    [HttpPost("config")]
    public async Task<IActionResult> UpdateConfig([FromBody] UpdateAdminConfigCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command, ct);
        return Ok(result);
    }
}
