using MarsBase.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MarsBase.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IGameService, GameService>();
        return services;
    }
}
