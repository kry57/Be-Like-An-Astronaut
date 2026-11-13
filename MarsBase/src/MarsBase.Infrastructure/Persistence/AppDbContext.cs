using MarsBase.Application.Abstractions;
using MarsBase.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarsBase.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<GameSession> Sessions => Set<GameSession>();
    public DbSet<ExplorerSession> ExplorerSessions => Set<ExplorerSession>();
    public DbSet<PlayerChoice> Choices => Set<PlayerChoice>();
    public DbSet<EmergencyEvent> Events => Set<EmergencyEvent>();
    public DbSet<EventOption> EventOptions => Set<EventOption>();
    public DbSet<OptionEffect> OptionEffects => Set<OptionEffect>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
