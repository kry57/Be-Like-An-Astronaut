using MarsBase.Api.Extensions;
using MarsBase.Application.Contracts;
using MarsBase.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarsBase.Api.Controllers;

/// <summary>API matching the six-level Little Galaxy Explorer HTML frontend.</summary>
[ApiController]
[Route("api/explorer/sessions")]
[Produces("application/json")]
public sealed class ExplorerSessionsController(IExplorerService explorer) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ExplorerStateDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Start([FromBody] StartExplorerRequest request, CancellationToken ct)
    {
        var result = await explorer.StartAsync(request, ct);
        return result.Match(this, dto => CreatedAtAction(nameof(Get), new { id = dto.Id }, dto));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var result = await explorer.GetAsync(id, ct);
        return result.Match(this, dto => Ok(dto));
    }

    [HttpPost("{id:guid}/decisions")]
    public async Task<IActionResult> RecordDecision(Guid id, [FromBody] ExplorerDecisionRequest request, CancellationToken ct)
    {
        var result = await explorer.RecordDecisionAsync(id, request, ct);
        return result.Match(this, dto => Ok(dto));
    }

    [HttpPatch("{id:guid}/screen")]
    public async Task<IActionResult> UpdateScreen(Guid id, [FromBody] ExplorerScreenRequest request, CancellationToken ct)
    {
        var result = await explorer.UpdateScreenAsync(id, request, ct);
        return result.Match(this, dto => Ok(dto));
    }

    [HttpPost("{id:guid}/restart")]
    public async Task<IActionResult> Restart(Guid id, CancellationToken ct)
    {
        var result = await explorer.RestartAsync(id, ct);
        return result.Match(this, dto => CreatedAtAction(nameof(Get), new { id = dto.Id }, dto));
    }
}
