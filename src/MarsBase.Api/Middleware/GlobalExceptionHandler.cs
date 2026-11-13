using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarsBase.Api.Middleware;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var isConcurrency = exception is DbUpdateConcurrencyException;

        if (!isConcurrency)
            logger.LogError(exception, "Unhandled exception while processing {Path}", httpContext.Request.Path);

        var status = isConcurrency ? StatusCodes.Status409Conflict : StatusCodes.Status500InternalServerError;
        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = isConcurrency ? "Concurrent update" : "Unexpected error",
                Detail = isConcurrency
                    ? "The session was modified by another request. Please retry."
                    : "An unexpected error occurred."
            }
        });
    }
}
