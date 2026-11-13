using MarsBase.Api.Extensions;
using MarsBase.Application.Contracts;
using MarsBase.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarsBase.Api.Controllers;

[ApiController]
[Route("api/sessions")]
[Produces("application/json")]
public sealed class SessionsController(IGameService game) : ControllerBase
{
    /// <summary>Starts a new 3-day mission (Day 01/03).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Start([FromBody] StartSessionRequest? request, CancellationToken ct)
    {
        var result = await game.StartAsync(request ?? new StartSessionRequest(), ct);
        return result.Match(this, dto => CreatedAtAction(nameof(Get), new { id = dto.Id }, dto));
    }

    /// <summary>Current state: resources, crew, build points, active emergency (or final result).</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var result = await game.GetAsync(id, ct);
        return result.Match(this, dto => Ok(dto));
    }

    /// <summary>Answers the current emergency by choosing one of its options ("Choose" button).</summary>
    [HttpPost("{id:guid}/choices")]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SubmitChoice(Guid id, [FromBody] SubmitChoiceRequest request, CancellationToken ct)
    {
        var result = await game.SubmitChoiceAsync(id, request, ct);
        return result.Match(this, dto => Ok(dto));
    }

    /// <summary>"Play Again": creates a fresh session for the same player.</summary>
    [HttpPost("{id:guid}/restart")]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Restart(Guid id, CancellationToken ct)
    {
        var result = await game.RestartAsync(id, ct);
        return result.Match(this, dto => CreatedAtAction(nameof(Get), new { id = dto.Id }, dto));
    }

    /// <summary>All decisions taken so far in this session.</summary>
    [HttpGet("{id:guid}/history")]
    [ProducesResponseType(typeof(IReadOnlyList<ChoiceHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> History(Guid id, CancellationToken ct)
    {
        var result = await game.GetHistoryAsync(id, ct);
        return result.Match(this, items => Ok(items));
    }
}
