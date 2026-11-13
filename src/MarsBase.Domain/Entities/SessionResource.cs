using MarsBase.Domain.Enums;

namespace MarsBase.Domain.Entities;

public class SessionResource
{
    public const int Min = 0;
    public const int Max = 100;

    private SessionResource() { }

    internal SessionResource(ResourceType type, int value)
    {
        Type = type;
        Value = Math.Clamp(value, Min, Max);
    }

    public int Id { get; private set; }
    public Guid SessionId { get; private set; }
    public ResourceType Type { get; private set; }
    public int Value { get; private set; }

    internal void Change(int delta) => Value = Math.Clamp(Value + delta, Min, Max);
}
