using MarsBase.Domain.Entities;
using MarsBase.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MarsBase.Infrastructure.Persistence;

/// <summary>
/// Seeds the 3 emergency events shown in the UI screens.
/// NOTE: the screens do not specify numeric effects, so the deltas below are
/// game-balance assumptions. Tweak them freely (or move them to the database).
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        if (await db.Events.AnyAsync(ct)) return;

        db.Events.AddRange(
            new EmergencyEvent
            {
                Day = 1,
                Title = "DUST STORM APPROACHING!",
                Description = "A powerful global dust storm is approaching! Visibility will drop, and equipment damage risk is high.",
                TimeLimitSeconds = 108, // IMPACT IN 01:48
                TimeoutPenalty = 8,
                Options =
                {
                    Option("Backup Batteries", "No effect on the other resources.", 0,
                        (ResourceType.SolarGrid, 6)),
                    Option("Small Nuclear Power Source", "Steady energy.", 0,
                        (ResourceType.SolarGrid, 10), (ResourceType.RadiationShielding, -8)),
                    Option("Power-Saving Mode", "Air drops a bit (less ventilation and heating).", 0,
                        (ResourceType.SolarGrid, 4), (ResourceType.Oxygen, -6))
                }
            },
            new EmergencyEvent
            {
                Day = 2,
                Title = "TOXIC SOIL!",
                Description = "Mars soil has a harmful chemical. Plants can't grow in it safely.",
                Options =
                {
                    Option("Soil Microbes", "Tiny helpers clean the soil, but slowly.", 0,
                        (ResourceType.Hydroponics, 4)),
                    Option("Wash the Soil", "Water removes the chemical, but uses water.", 0,
                        (ResourceType.Hydroponics, 12), (ResourceType.Water, -15)),
                    Option("Grow Without Soil", "Plants grow in water, but you need equipment.", 3,
                        (ResourceType.Hydroponics, 10))
                }
            },
            new EmergencyEvent
            {
                Day = 3,
                Title = "COOLING LOOP LEAK!",
                Description = "Liquid is leaking from the cooling pipes. Immediate action required.",
                Options =
                {
                    Option("Quick Patch", "Seal the leak fast, but it might come back.", 0,
                        (ResourceType.Water, -4)),
                    Option("Install New Pipes", "A permanent fix, but it costs Build Points.", 4,
                        (ResourceType.Water, 4), (ResourceType.SolarGrid, 2)),
                    Option("Save Power, Fix Later", "Use less energy now, but the leak keeps going.", 0,
                        (ResourceType.SolarGrid, 6), (ResourceType.Water, -12))
                }
            });

        await db.SaveChangesAsync(ct);
    }

    private static EventOption Option(
        string title, string description, int buildPointsCost,
        params (ResourceType Resource, int Delta)[] effects)
        => new()
        {
            Title = title,
            Description = description,
            BuildPointsCost = buildPointsCost,
            Effects = effects
                .Select(e => new OptionEffect { Resource = e.Resource, Delta = e.Delta })
                .ToList()
        };
}
