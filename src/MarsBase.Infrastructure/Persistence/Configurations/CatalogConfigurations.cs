using MarsBase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarsBase.Infrastructure.Persistence.Configurations;

internal sealed class EmergencyEventConfiguration : IEntityTypeConfiguration<EmergencyEvent>
{
    public void Configure(EntityTypeBuilder<EmergencyEvent> builder)
    {
        builder.ToTable("EmergencyEvents");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500).IsRequired();
        builder.HasIndex(e => e.Day).IsUnique();

        builder.HasMany(e => e.Options)
            .WithOne()
            .HasForeignKey(o => o.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class EventOptionConfiguration : IEntityTypeConfiguration<EventOption>
{
    public void Configure(EntityTypeBuilder<EventOption> builder)
    {
        builder.ToTable("EventOptions");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Title).HasMaxLength(100).IsRequired();
        builder.Property(o => o.Description).HasMaxLength(300).IsRequired();

        builder.HasMany(o => o.Effects)
            .WithOne()
            .HasForeignKey(e => e.OptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class OptionEffectConfiguration : IEntityTypeConfiguration<OptionEffect>
{
    public void Configure(EntityTypeBuilder<OptionEffect> builder)
    {
        builder.ToTable("OptionEffects");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Resource).HasConversion<string>().HasMaxLength(30).IsRequired();
    }
}
