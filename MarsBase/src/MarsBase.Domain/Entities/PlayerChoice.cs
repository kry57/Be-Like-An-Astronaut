namespace MarsBase.Domain.Entities;

public class PlayerChoice
{
    private PlayerChoice() { }

    internal PlayerChoice(int day, int eventId, int optionId, bool wasLate, DateTime madeAtUtc)
    {
        Day = day;
        EventId = eventId;
        OptionId = optionId;
        WasLate = wasLate;
        MadeAtUtc = madeAtUtc;
    }

    public int Id { get; private set; }
    public Guid SessionId { get; private set; }
    public int Day { get; private set; }

    public int EventId { get; private set; }
    public EmergencyEvent Event { get; private set; } = null!;

    public int OptionId { get; private set; }
    public EventOption Option { get; private set; } = null!;

    /// <summary>True when the player answered after the event countdown expired.</summary>
    public bool WasLate { get; private set; }
    public DateTime MadeAtUtc { get; private set; }
}
