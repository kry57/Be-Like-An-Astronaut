using MarsBase.Application.Contracts;
using MarsBase.Domain.Entities;

namespace MarsBase.Application.Mappings;

internal static class EventMappings
{
    public static EventDto ToDto(this EmergencyEvent e, DateTime startedAtUtc, DateTime nowUtc)
    {
        int? remaining = e.TimeLimitSeconds is int limit
            ? Math.Max(0, (int)Math.Ceiling(limit - (nowUtc - startedAtUtc).TotalSeconds))
            : null;

        var options = e.Options
            .OrderBy(o => o.Id)
            .Select(o => new OptionDto(o.Id, o.Title, o.Description, o.BuildPointsCost))
            .ToList();

        return new EventDto(e.Id, e.Day, e.Title, e.Description, e.TimeLimitSeconds, remaining, options);
    }
}
