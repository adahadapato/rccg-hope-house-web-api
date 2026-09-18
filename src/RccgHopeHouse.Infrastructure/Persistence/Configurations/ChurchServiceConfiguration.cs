using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for <see cref="ChurchService"/> schedule and Zoom metadata.
/// </summary>
public class ChurchServiceConfiguration : IEntityTypeConfiguration<ChurchService>
{
    public void Configure(EntityTypeBuilder<ChurchService> builder)
    {
        builder.ToTable("ChurchServices");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Description).HasMaxLength(500);
        builder.Property(s => s.Location).HasMaxLength(100);
        builder.Property(s => s.ZoomId).HasMaxLength(50);
        builder.Property(s => s.ZoomPasscode).HasMaxLength(20);

        // Indexes for public schedule queries
        builder.HasIndex(s => s.DayOfWeek);
        builder.HasIndex(s => s.IsActive);
        builder.HasIndex(s => s.DisplayOrder);

        // Supports filtering the public feed by local vs. HQ-broadcast
        // services (RegularServices.tsx vs. MonthlyServices.tsx query
        // separately by this flag).
        builder.HasIndex(s => s.IsLocal);
    }
}