using MarsBase.Application.Common;
using MarsBase.Application.Contracts;

namespace MarsBase.Application.Services;

public interface IGameService
{
    Task<Result<SessionDto>> StartAsync(StartSessionRequest request, CancellationToken ct);
    Task<Result<SessionDto>> GetAsync(Guid sessionId, CancellationToken ct);
    Task<Result<SessionDto>> SubmitChoiceAsync(Guid sessionId, SubmitChoiceRequest request, CancellationToken ct);
    Task<Result<SessionDto>> RestartAsync(Guid sessionId, CancellationToken ct);
    Task<Result<IReadOnlyList<ChoiceHistoryDto>>> GetHistoryAsync(Guid sessionId, CancellationToken ct);
}
