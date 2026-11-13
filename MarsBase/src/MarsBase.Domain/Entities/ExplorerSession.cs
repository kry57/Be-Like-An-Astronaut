namespace MarsBase.Domain.Entities;

/// <summary>Persistent state for the six-level Little Galaxy Explorer frontend.</summary>
public sealed class ExplorerSession
{
    public Guid Id { get; set; }
    public string PlayerName { get; set; } = "Space Explorer";
    public string Hero { get; set; } = "adam";
    public string PlayMode { get; set; } = "solo";
    public string Difficulty { get; set; } = "easy";
    public string Status { get; set; } = "InProgress";
    public string CurrentScreenId { get; set; } = "1";
    public int CurrentLevel { get; set; } = 1;
    public int ActivePlayerIndex { get; set; }
    public int TotalXp { get; set; }
    public int O2 { get; set; } = 94;
    public int Water { get; set; } = 88;
    public int Solar { get; set; } = 91;
    public int Rations { get; set; } = 76;
    public int Shield { get; set; } = 98;
    public int Health { get; set; } = 92;
    public string StarsJson { get; set; } = "{\"1\":0,\"2\":0,\"3\":0,\"4\":0,\"5\":0,\"6\":0}";
    public string PlayersJson { get; set; } = "[]";
    public string HistoryJson { get; set; } = "[]";
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
