
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for daily devotionals.
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

        builder.Property(d => d.MemoryVerseReference)
            .IsRequired()
            .HasMaxLength(250)
            .HasDefaultValue("");

        builder.Property(d => d.MemoryVersePassageId)
            .IsRequired()
            .HasMaxLength(250)
            .HasDefaultValue("");

        builder.Property(d => d.BibleInOneYearReference)
            .IsRequired()
            .HasMaxLength(1000)
            .HasDefaultValue("");

        builder.Property(d => d.BibleInOneYearPassageIdsJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)")
            .HasDefaultValue("[]");

        builder.Property(d => d.Thought)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(d => d.CommentaryJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(d => d.PrayerPointsJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(d => d.Declaration)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(d => d.HymnNumber)
                .IsRequired(false)
                .HasMaxLength(50);

        builder.Property(d => d.HymnTitle)
            .IsRequired(false)
            .HasMaxLength(300);

        builder.Property(d => d.HymnLyrics)
            .IsRequired(false)
            .HasColumnType("nvarchar(max)");

        builder.Property(d => d.AdditionalReading)
            .IsRequired()
            .HasColumnType("nvarchar(max)")
            .HasDefaultValue("");

        builder.Property(d => d.KeyPoint)
            .IsRequired()
            .HasMaxLength(2000)
            .HasDefaultValue("");

        builder.Property(d => d.Author)
            .IsRequired()
            .HasMaxLength(200)
            .HasDefaultValue("");

        builder.Property(d => d.SourceUrl)
            .IsRequired()
            .HasMaxLength(2000)
            .HasDefaultValue("");

        builder.Property(d => d.IsPublished)
            .IsRequired();

        builder.Property(d => d.PublishedAt);

        // Prevent duplicate devotionals for the same date.
        builder.HasIndex(d => d.DevotionalDate)
            .IsUnique();

        // Optimise published devotional retrieval.
        builder.HasIndex(d => new
        {
            d.IsPublished,
            d.DevotionalDate
        });
    }
}
