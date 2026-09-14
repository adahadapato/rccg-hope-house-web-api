using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for <see cref="ThanksgivingService"/> monthly video records.
/// </summary>
public class ThanksgivingServiceConfiguration : IEntityTypeConfiguration<ThanksgivingService>
{
    public void Configure(EntityTypeBuilder<ThanksgivingService> builder)
    {
        builder.ToTable("ThanksgivingServices");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.VideoUrl).IsRequired().HasMaxLength(500);
        //builder.Property(t => t.ThumbnailUrl).HasMaxLength(500);
        builder.Property(t => t.Description).HasMaxLength(1000);

        // Index for chronological monthly feed queries
        builder.HasIndex(t => t.ServiceMonth);
        builder.HasIndex(t => t.IsAutoSynced);
    }
}