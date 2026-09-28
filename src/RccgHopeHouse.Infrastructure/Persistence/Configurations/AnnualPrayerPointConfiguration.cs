using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for <see cref="AnnualPrayerPoint"/> — an individual,
/// ordered prayer point belonging to an annual prayer.
/// </summary>
public class AnnualPrayerPointConfiguration
    : IEntityTypeConfiguration<AnnualPrayerPoint>
{
    public void Configure(
        EntityTypeBuilder<AnnualPrayerPoint> builder)
    {
        builder.ToTable("AnnualPrayerPoint");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.AnnualPrayerId)
            .IsRequired();

        builder.Property(p => p.Text)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(p => p.DisplayOrder)
            .IsRequired();

        builder.HasIndex(
                p => new
                {
                    p.AnnualPrayerId,
                    p.DisplayOrder
                })
            .IsUnique();

        builder.HasIndex(p => p.AnnualPrayerId);
    }
}