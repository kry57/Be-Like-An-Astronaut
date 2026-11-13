using MarsBase.Domain.Enums;

namespace MarsBase.Domain.Entities;

/// <summary>
/// Aggregate root: one playthrough of the 3-day Mars Base Alpha mission.
/// All game rules that mutate state live here so they cannot be bypassed.
/// </summary>
public class GameSession
{
    public const int DefaultTotalDays = 3;
    public const int InitialBuildPoints = 5;
    public const string DefaultPlayerName = "Space Explorer";

    // Starting values taken from the UI screens.
    private static readonly (ResourceType Type, int Value)[] InitialResources =
    [
        (ResourceType.Oxygen, 94),
        (ResourceType.Water, 88),
        (ResourceType.SolarGrid, 91),
        (ResourceType.Hydroponics, 76),
        (ResourceType.RadiationShielding, 98),
        (ResourceType.CrewHealth, 92)
    ];

    private readonly List<SessionResource> _resources = [];
    private readonly List<PlayerChoice> _choices = [];

    private GameSession() { }

    public Guid Id { get; private set; }
    public string PlayerName { get; private set; } = null!;
    public GameStatus Status { get; private set; }
    public int CurrentDay { get; private set; }
    public int TotalDays { get; private set; }
    public int BuildPoints { get; private set; }
    public DateTime CurrentEventStartedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public IReadOnlyCollection<SessionResource> Resources => _resources;
    public IReadOnlyCollection<PlayerChoice> Choices => _choices;

    public bool IsInProgress => Status == GameStatus.InProgress;

    public static GameSession Start(string? playerName, DateTime nowUtc)
    {
        var session = new GameSession
        {
            Id = Guid.CreateVersion7(),
            PlayerName = string.IsNullOrWhiteSpace(playerName) ? DefaultPlayerName : playerName.Trim(),
            Status = GameStatus.InProgress,
            CurrentDay = 1,
            TotalDays = DefaultTotalDays,
            BuildPoints = InitialBuildPoints,
            CurrentEventStartedAtUtc = nowUtc,
            CreatedAtUtc = nowUtc
        };

        foreach (var (type, value) in InitialResources)
            session._resources.Add(new SessionResource(type, value));

        return session;
    }

    public int GetResource(ResourceType type) => _resources.First(r => r.Type == type).Value;

    public bool TrySpendBuildPoints(int cost)
    {
        if (cost <= 0) return true;
        if (cost > BuildPoints) return false;
        BuildPoints -= cost;
        return true;
    }

    public void ApplyDelta(ResourceType type, int delta)
        => _resources.First(r => r.Type == type).Change(delta);

    /// <summary>Late answers damage equipment-related resources (not oxygen, radiation or crew directly).</summary>
    public void ApplyTimeoutPenalty(int penalty)
    {
        if (penalty <= 0) return;
        ApplyDelta(ResourceType.Water, -penalty);
        ApplyDelta(ResourceType.SolarGrid, -penalty);
        ApplyDelta(ResourceType.Hydroponics, -penalty);
    }

    public void RecordChoice(int eventId, int optionId, bool wasLate, DateTime nowUtc)
        => _choices.Add(new PlayerChoice(CurrentDay, eventId, optionId, wasLate, nowUtc));

    /// <summary>Ends the mission as failed, completed, or moves to the next day.</summary>
    public void AdvanceDay(DateTime nowUtc)
    {
        if (GetResource(ResourceType.Oxygen) == 0 || GetResource(ResourceType.CrewHealth) == 0)
        {
            Finish(GameStatus.Failed, nowUtc);
            return;
        }

        if (CurrentDay >= TotalDays)
        {
            Finish(GameStatus.Completed, nowUtc);
            return;
        }

        CurrentDay++;
        CurrentEventStartedAtUtc = nowUtc;
    }

    /// <summary>Average of all resources (0-100).</summary>
    public int CalculateScore() => (int)Math.Round(_resources.Average(r => r.Value));

    private void Finish(GameStatus status, DateTime nowUtc)
    {
        Status = status;
        CompletedAtUtc = nowUtc;
    }
}
