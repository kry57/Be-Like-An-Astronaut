using MarsBase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarsBase.Infrastructure.Persistence.Configurations;

internal sealed class GameSessionConfiguration : IEntityTypeConfiguration<GameSession>
{
    public void Configure(EntityTypeBuilder<GameSession> builder)
    {
        builder.ToTable("GameSessions");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.PlayerName).HasMaxLength(40).IsRequired();
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(s => s.RowVersion).IsRowVersion();

        builder.Ignore(s => s.IsInProgress);

        builder.HasMany(s => s.Resources)
            .WithOne()
            .HasForeignKey(r => r.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Choices)
            .WithOne()
            .HasForeignKey(c => c.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.Resources).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(s => s.Choices).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

internal sealed class SessionResourceConfiguration : IEntityTypeConfiguration<SessionResource>
{
    public void Configure(EntityTypeBuilder<SessionResource> builder)
    {
        builder.ToTable("SessionResources");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Type).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.HasIndex(r => new { r.SessionId, r.Type }).IsUnique();
    }
}

internal sealed class PlayerChoiceConfiguration : IEntityTypeConfiguration<PlayerChoice>
{
    public void Configure(EntityTypeBuilder<PlayerChoice> builder)
    {
        builder.ToTable("PlayerChoices");
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.Event)
            .WithMany()
            .HasForeignKey(c => c.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Option)
            .WithMany()
            .HasForeignKey(c => c.OptionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.SessionId, c.Day }).IsUnique();
    }
}
