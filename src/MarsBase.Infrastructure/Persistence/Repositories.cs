using MarsBase.Application.Abstractions;
using MarsBase.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarsBase.Infrastructure.Persistence;

internal sealed class GameSessionRepository(AppDbContext db) : IGameSessionRepository
{
    public Task<GameSession?> GetByIdAsync(Guid id, bool track, CancellationToken ct)
    {
        IQueryable<GameSession> query = db.Sessions.Include(s => s.Resources);
        if (!track) query = query.AsNoTracking();

        return query.FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct)
        => db.Sessions.AnyAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<PlayerChoice>> GetChoicesAsync(Guid sessionId, CancellationToken ct)
        => await db.Choices
            .AsNoTracking()
            .Include(c => c.Event)
            .Include(c => c.Option)
            .Where(c => c.SessionId == sessionId)
            .OrderBy(c => c.Day)
            .ToListAsync(ct);

    public async Task AddAsync(GameSession session, CancellationToken ct)
        => await db.Sessions.AddAsync(session, ct);
}

internal sealed class EmergencyEventRepository(AppDbContext db) : IEmergencyEventRepository
{
    public Task<EmergencyEvent?> GetByDayAsync(int day, CancellationToken ct)
        => db.Events
            .AsNoTracking()
            .Include(e => e.Options)
            .ThenInclude(o => o.Effects)
            .AsSplitQuery()
            .FirstOrDefaultAsync(e => e.Day == day, ct);
}
