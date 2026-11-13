namespace MarsBase.Domain.Entities;

/// <summary>Catalog entity: one decision the player can take for an event.</summary>
public class EventOption
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int BuildPointsCost { get; set; }

    public ICollection<OptionEffect> Effects { get; set; } = new List<OptionEffect>();
}
