using MarsBase.Application.Abstractions;
using MarsBase.Infrastructure.Persistence;
using MarsBase.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarsBase.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IGameSessionRepository, GameSessionRepository>();
        services.AddScoped<IEmergencyEventRepository, EmergencyEventRepository>();
        services.AddScoped<IExplorerSessionRepository, ExplorerSessionRepository>();
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }

    /// <summary>
    /// Creates the schema (if missing) and seeds the event catalog.
    /// For production, replace EnsureCreated with EF Core migrations (see README).
    /// </summary>
    public static async Task InitializeDatabaseAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.EnsureCreatedAsync(ct);
        await DbSeeder.SeedAsync(db, ct);
    }
}
