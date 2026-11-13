namespace MarsBase.Domain.Entities;

/// <summary>Catalog entity: the emergency the player faces on a given day.</summary>
public class EmergencyEvent
{
    public int Id { get; set; }
    public int Day { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;

    /// <summary>Optional countdown (e.g. 108 seconds = 01:48 for the dust storm).</summary>
    public int? TimeLimitSeconds { get; set; }

    /// <summary>Damage applied to equipment resources when the player answers after the countdown.</summary>
    public int TimeoutPenalty { get; set; }

    public ICollection<EventOption> Options { get; set; } = new List<EventOption>();
}
