using MarsBase.Domain.Entities;

namespace MarsBase.Application.Abstractions;

public interface IGameSessionRepository
{
    /// <summary>Loads a session with its resources. Use track = true when the session will be modified.</summary>
    Task<GameSession?> GetByIdAsync(Guid id, bool track, CancellationToken ct);

    Task<bool> ExistsAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<PlayerChoice>> GetChoicesAsync(Guid sessionId, CancellationToken ct);

    Task AddAsync(GameSession session, CancellationToken ct);
}

public interface IEmergencyEventRepository
{
    /// <summary>Returns the event for a day including its options and effects (no tracking).</summary>
    Task<EmergencyEvent?> GetByDayAsync(int day, CancellationToken ct);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public interface IClock
{
    DateTime UtcNow { get; }
}
