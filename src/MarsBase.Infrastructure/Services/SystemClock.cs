using MarsBase.Application.Abstractions;

namespace MarsBase.Infrastructure.Services;

internal sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
