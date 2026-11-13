using MarsBase.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace MarsBase.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult Match<T>(this Result<T> result, ControllerBase controller, Func<T, IActionResult> onSuccess)
        => result.IsSuccess ? onSuccess(result.Value) : result.Error.ToProblem(controller);

    public static IActionResult ToProblem(this Error error, ControllerBase controller)
    {
        var status = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return controller.Problem(title: error.Code, detail: error.Description, statusCode: status);
    }
}
