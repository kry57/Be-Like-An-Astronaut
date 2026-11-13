using MarsBase.Domain.Enums;

namespace MarsBase.Domain.Entities;

/// <summary>Catalog entity: how an option changes one resource (delta can be negative).</summary>
public class OptionEffect
{
    public int Id { get; set; }
    public int OptionId { get; set; }
    public ResourceType Resource { get; set; }
    public int Delta { get; set; }
}
