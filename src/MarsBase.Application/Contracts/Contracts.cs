using System.ComponentModel.DataAnnotations;

namespace MarsBase.Application.Contracts;

// ---------- Requests ----------

public sealed record StartSessionRequest
{
    [StringLength(40)]
    public string? PlayerName { get; init; }
}

public sealed record SubmitChoiceRequest
{
    [Range(1, int.MaxValue)]
    public int OptionId { get; init; }
}

// ---------- Responses ----------

public sealed record ResourceDto(string Key, string Name, int Value);

public sealed record CrewDto(int Active, int Total, int Vitals);

public sealed record OptionDto(int Id, string Title, string Description, int BuildPointsCost);

public sealed record EventDto(
    int Id,
    int Day,
    string Title,
    string Description,
    int? TimeLimitSeconds,
    int? SecondsRemaining,
    IReadOnlyList<OptionDto> Options);

public sealed record MissionResultDto(
    bool Success,
    string Title,
    string Message,
    int Score,
    IReadOnlyList<string> Lessons);

public sealed record SessionDto(
    Guid Id,
    string PlayerName,
    string Status,
    int CurrentDay,
    int TotalDays,
    int BuildPoints,
    IReadOnlyList<ResourceDto> Resources,
    CrewDto Crew,
    EventDto? CurrentEvent,
    MissionResultDto? Result);

public sealed record ChoiceHistoryDto(
    int Day,
    string EventTitle,
    string OptionTitle,
    bool WasLate,
    DateTime MadeAtUtc);
