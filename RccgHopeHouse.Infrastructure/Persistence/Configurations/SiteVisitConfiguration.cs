using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for <see cref="SiteVisit"/>: one anonymous
/// website visitor per day.
/// </summary>
public class SiteVisitConfiguration : IEntityTypeConfiguration<SiteVisit>
{
    public void Configure(EntityTypeBuilder<SiteVisit> builder)
    {
        builder.ToTable("SiteVisits");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.VisitorHash)
            .IsRequired()
            .HasMaxLength(SiteVisit.VisitorHashLength)
            .IsUnicode(false);

        // One row per visitor per day. Also used for every lookup
        // and for the daily totals.
        builder.HasIndex(v => new { v.VisitDate, v.VisitorHash })
            .IsUnique();

        // "Online now" looks for recently seen visitors.
        builder.HasIndex(v => v.LastSeenAt);
    }
}
