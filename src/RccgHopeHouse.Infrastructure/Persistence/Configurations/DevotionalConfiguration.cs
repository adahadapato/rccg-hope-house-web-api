using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Devotional"/> entity.
/// Configures devotional content, publication state, JSON-backed ordered
/// commentary/prayer points, and date-based public/admin querying.
/// </summary>
public class DevotionalConfiguration : IEntityTypeConfiguration<Devotional>
{
    public void Configure(EntityTypeBuilder<Devotional> builder)
    {
        builder.ToTable("Devotionals");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.DevotionalDate)
            .IsRequired();

        builder.Property(d => d.Theme)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.ScriptureReference)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(d => d.PassageId)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(d => d.Thought)
            .IsRequired()
            .HasMaxLength(2000);

        // Ordered commentary points are serialized as JSON.
        // No arbitrary max length because devotional commentary
        // may contain substantial written content.
        builder.Property(d => d.CommentaryJson)
            .IsRequired();

        // Ordered prayer points are serialized as JSON.
        builder.Property(d => d.PrayerPointsJson)
            .IsRequired();

        builder.Property(d => d.Declaration)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(d => d.IsPublished)
            .IsRequired();

        builder.Property(d => d.PublishedAt);

        // There can only be one devotional assigned to a calendar date.
        // This provides database-level protection in addition to the
        // application/repository duplicate-date checks.
        builder.HasIndex(d => d.DevotionalDate)
            .IsUnique();

        // Supports public queries such as:
        // IsPublished = true AND DevotionalDate <= today,
        // ordered by DevotionalDate descending.
        builder.HasIndex(d => new
        {
            d.IsPublished,
            d.DevotionalDate
        });
    }
}