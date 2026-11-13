using MarsBase.Application.Common;
using MarsBase.Application.Contracts;

namespace MarsBase.Application.Services;

public interface IExplorerService
{
    Task<Result<ExplorerStateDto>> StartAsync(StartExplorerRequest request, CancellationToken ct);
    Task<Result<ExplorerStateDto>> GetAsync(Guid id, CancellationToken ct);
    Task<Result<ExplorerStateDto>> RecordDecisionAsync(Guid id, ExplorerDecisionRequest request, CancellationToken ct);
    Task<Result<ExplorerStateDto>> UpdateScreenAsync(Guid id, ExplorerScreenRequest request, CancellationToken ct);
    Task<Result<ExplorerStateDto>> RestartAsync(Guid id, CancellationToken ct);
}
