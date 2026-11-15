using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for <see cref="AnnualPrayer"/> — the annual prayer content,
/// scripture, declaration, and ordered prayer points shown on the homepage.
/// </summary>
public class AnnualPrayerConfiguration
    : IEntityTypeConfiguration<AnnualPrayer>
{
    public void Configure(
        EntityTypeBuilder<AnnualPrayer> builder)
    {
        builder.ToTable("AnnualPrayer");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Year)
            .IsRequired();

        builder.Property(p => p.Theme)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Service)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(p => p.Author)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.BibleReference)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.BibleText)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(p => p.Declaration)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(p => p.ClosingVerse)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(1000);

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.HasIndex(p => p.Year)
            .IsUnique();

        builder.HasMany(p => p.PrayerPoints)
            .WithOne(p => p.AnnualPrayer)
            .HasForeignKey(p => p.AnnualPrayerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}