using MarsBase.Domain.Enums;

namespace MarsBase.Application.Common;

public static class ResourceTypeExtensions
{
    /// <summary>Labels exactly as shown in the UI screens.</summary>
    public static string DisplayName(this ResourceType type) => type switch
    {
        ResourceType.Oxygen => "Oxygen Cycle",
        ResourceType.Water => "Recycled Water",
        ResourceType.SolarGrid => "Solar Grid Reserves",
        ResourceType.Hydroponics => "Hydroponics Rations",
        ResourceType.RadiationShielding => "Radiation Shielding",
        ResourceType.CrewHealth => "Vitals/Health",
        _ => type.ToString()
    };
}
