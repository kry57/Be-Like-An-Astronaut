namespace MarsBase.Application.Common;

public static class GameErrors
{
    public static readonly Error SessionNotFound =
        Error.NotFound("Session.NotFound", "The game session was not found.");

    public static readonly Error SessionFinished =
        Error.Conflict("Session.Finished", "The mission is already finished. Restart to play again.");

    public static readonly Error EventNotFound =
        Error.NotFound("Event.NotFound", "No emergency event is configured for the current day.");

    public static readonly Error InvalidOption =
        Error.Validation("Option.Invalid", "The selected option does not belong to the current emergency.");

    public static readonly Error InsufficientBuildPoints =
        Error.Conflict("BuildPoints.Insufficient", "Not enough Build Points for this option.");
}

