using MarsBase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarsBase.Infrastructure.Persistence.Configurations;

internal sealed class ExplorerSessionConfiguration : IEntityTypeConfiguration<ExplorerSession>
{
    public void Configure(EntityTypeBuilder<ExplorerSession> builder)
    {
        builder.ToTable("ExplorerSessions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PlayerName).HasMaxLength(40).IsRequired();
        builder.Property(x => x.Hero).HasMaxLength(12).IsRequired();
        builder.Property(x => x.PlayMode).HasMaxLength(12).IsRequired();
        builder.Property(x => x.Difficulty).HasMaxLength(12).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CurrentScreenId).HasMaxLength(8).IsRequired();
        builder.Property(x => x.StarsJson).IsRequired();
        builder.Property(x => x.PlayersJson).IsRequired();
        builder.Property(x => x.HistoryJson).IsRequired();
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}
