using MarsBase.Application.Abstractions;
using MarsBase.Application.Common;
using MarsBase.Application.Contracts;
using MarsBase.Application.Mappings;
using MarsBase.Domain.Entities;
using MarsBase.Domain.Enums;

namespace MarsBase.Application.Services;

public sealed class GameService(
    IGameSessionRepository sessions,
    IEmergencyEventRepository events,
    IUnitOfWork unitOfWork,
    IClock clock) : IGameService
{
    private const int CrewTotal = 4;

    // "What You Learned" section of the Mission Complete screen.
    private static readonly string[] Lessons =
    [
        "You can always balance resources and safety.",
        "Every decision has a cost and benefit.",
        "Sometimes the safest choice is not the easiest.",
        "Small steps make a big difference."
    ];

    public async Task<Result<SessionDto>> StartAsync(StartSessionRequest request, CancellationToken ct)
    {
        var session = GameSession.Start(request.PlayerName, clock.UtcNow);
        await sessions.AddAsync(session, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<SessionDto>.Success(await BuildDtoAsync(session, ct));
    }

    public async Task<Result<SessionDto>> GetAsync(Guid sessionId, CancellationToken ct)
    {
        var session = await sessions.GetByIdAsync(sessionId, track: false, ct);
        if (session is null) return GameErrors.SessionNotFound;

        return Result<SessionDto>.Success(await BuildDtoAsync(session, ct));
    }

    public async Task<Result<SessionDto>> SubmitChoiceAsync(
        Guid sessionId, SubmitChoiceRequest request, CancellationToken ct)
    {
        var session = await sessions.GetByIdAsync(sessionId, track: true, ct);
        if (session is null) return GameErrors.SessionNotFound;
        if (!session.IsInProgress) return GameErrors.SessionFinished;

        var emergency = await events.GetByDayAsync(session.CurrentDay, ct);
        if (emergency is null) return GameErrors.EventNotFound;

        var option = emergency.Options.FirstOrDefault(o => o.Id == request.OptionId);
        if (option is null) return GameErrors.InvalidOption;

        if (!session.TrySpendBuildPoints(option.BuildPointsCost))
            return GameErrors.InsufficientBuildPoints;

        var now = clock.UtcNow;
        var wasLate = emergency.TimeLimitSeconds is int limit
                      && now > session.CurrentEventStartedAtUtc.AddSeconds(limit);

        foreach (var effect in option.Effects)
            session.ApplyDelta(effect.Resource, effect.Delta);

        if (wasLate)
            session.ApplyTimeoutPenalty(emergency.TimeoutPenalty);

        session.RecordChoice(emergency.Id, option.Id, wasLate, now);
        session.AdvanceDay(now);

        await unitOfWork.SaveChangesAsync(ct);

        return Result<SessionDto>.Success(await BuildDtoAsync(session, ct));
    }

    public async Task<Result<SessionDto>> RestartAsync(Guid sessionId, CancellationToken ct)
    {
        var previous = await sessions.GetByIdAsync(sessionId, track: false, ct);
        if (previous is null) return GameErrors.SessionNotFound;

        var fresh = GameSession.Start(previous.PlayerName, clock.UtcNow);
        await sessions.AddAsync(fresh, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<SessionDto>.Success(await BuildDtoAsync(fresh, ct));
    }

    public async Task<Result<IReadOnlyList<ChoiceHistoryDto>>> GetHistoryAsync(Guid sessionId, CancellationToken ct)
    {
        if (!await sessions.ExistsAsync(sessionId, ct))
            return GameErrors.SessionNotFound;

        var choices = await sessions.GetChoicesAsync(sessionId, ct);
        IReadOnlyList<ChoiceHistoryDto> history = choices
            .Select(c => new ChoiceHistoryDto(c.Day, c.Event.Title, c.Option.Title, c.WasLate, c.MadeAtUtc))
            .ToList();

        return Result<IReadOnlyList<ChoiceHistoryDto>>.Success(history);
    }

    private async Task<SessionDto> BuildDtoAsync(GameSession session, CancellationToken ct)
    {
        EventDto? currentEvent = null;
        MissionResultDto? result = null;

        if (session.IsInProgress)
        {
            var emergency = await events.GetByDayAsync(session.CurrentDay, ct);
            currentEvent = emergency?.ToDto(session.CurrentEventStartedAtUtc, clock.UtcNow);
        }
        else
        {
            result = BuildResult(session);
        }

        var resources = session.Resources
            .Where(r => r.Type != ResourceType.CrewHealth)
            .OrderBy(r => r.Type)
            .Select(r => new ResourceDto(r.Type.ToString(), r.Type.DisplayName(), r.Value))
            .ToList();

        var crew = new CrewDto(CrewTotal, CrewTotal, session.GetResource(ResourceType.CrewHealth));

        return new SessionDto(
            session.Id,
            session.PlayerName,
            session.Status.ToString(),
            session.CurrentDay,
            session.TotalDays,
            session.BuildPoints,
            resources,
            crew,
            currentEvent,
            result);
    }

    private static MissionResultDto BuildResult(GameSession session)
    {
        var score = session.CalculateScore();

        return session.Status == GameStatus.Completed
            ? new MissionResultDto(
                true,
                "Mission Complete!",
                $"You kept your crew safe for {session.TotalDays} days! Great work, {session.PlayerName}!",
                score,
                Lessons)
            : new MissionResultDto(
                false,
                "Mission Failed",
                "The base could not survive. Try a different strategy.",
                score,
                Lessons);
    }
}
