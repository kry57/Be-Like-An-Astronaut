using System.ComponentModel.DataAnnotations;

namespace MarsBase.Application.Contracts;

public sealed record ExplorerPlayerRequest(
    [property: Required, StringLength(40, MinimumLength = 1)] string Name,
    [property: Range(4, 99)] int Age,
    string? Role = null);

public sealed record StartExplorerRequest
{
    [StringLength(40)]
    public string? PlayerName { get; init; }
    public string Hero { get; init; } = "adam";
    public string PlayMode { get; init; } = "solo";
    public string Difficulty { get; init; } = "easy";
    public IReadOnlyList<ExplorerPlayerRequest>? Players { get; init; }
}

public sealed record ExplorerDecisionRequest
{
    [Range(1, 6)] public int Level { get; init; }
    [Range(1, 3)] public int Option { get; init; }
}

public sealed record ExplorerScreenRequest
{
    [Required, StringLength(8)] public string ScreenId { get; init; } = "1";
    [Range(1, 6)] public int Level { get; init; } = 1;
    public bool AdvanceTurn { get; init; }
}

public sealed record ExplorerPlayerDto(string Name, int Age, string Role);
public sealed record ExplorerTelemetryDto(int O2, int Water, int Solar, int Rations, int Shield, int Health);
public sealed record ExplorerDecisionDto(int Level, int Option, string Title, string Lesson, int GainedXp, int Stars);
public sealed record ExplorerStateDto(
    Guid Id, string PlayerName, string Hero, string PlayMode, string Difficulty, string Status,
    string CurrentScreenId, int CurrentLevel, int ActivePlayerIndex, ExplorerPlayerDto ActivePlayer,
    IReadOnlyList<ExplorerPlayerDto> Players, int TotalXp, int TotalStars,
    IReadOnlyDictionary<string, int> LevelStars, ExplorerTelemetryDto Telemetry,
    IReadOnlyList<ExplorerDecisionDto> History);
