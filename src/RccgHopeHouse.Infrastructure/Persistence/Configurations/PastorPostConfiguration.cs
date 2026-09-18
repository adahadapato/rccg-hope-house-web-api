using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="PastorPost"/> entity.
/// Handles rich HTML content, binary cover images, the required relationship
/// to ThemeOfTheYear, and feed-optimized indexing.
/// </summary>
public class PastorPostConfiguration : IEntityTypeConfiguration<PastorPost>
{
    public void Configure(EntityTypeBuilder<PastorPost> builder)
    {
        builder.ToTable("PastorPosts");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Content).IsRequired(); // HTML content; no arbitrary max length
        builder.Property(p => p.Excerpt).HasMaxLength(500);
        builder.Property(p => p.AuthorName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.BibleReference).HasMaxLength(100);

        // Store cover images as VARBINARY(MAX) per architectural decision
        builder.Property(p => p.CoverImageData).HasColumnType("VARBINARY(MAX)");
        builder.Property(p => p.CoverImageContentType).HasMaxLength(50);

        // Required relationship: every PastorPost belongs to exactly one
        // year's theme. Restrict delete — deleting a ThemeOfTheYear must not
        // silently cascade-delete every article ever taught under it; an
        // admin must reassign or explicitly remove those posts first.
        builder.Property(p => p.ThemeOfTheYearId).IsRequired();

        builder.HasOne(p => p.ThemeOfTheYear)
               .WithMany(t => t.Posts)
               .HasForeignKey(p => p.ThemeOfTheYearId)
               .OnDelete(DeleteBehavior.Restrict);

        // Indexes for feed queries, admin filtering, chronological sorting,
        // and fetching sibling articles under the same theme.
        builder.HasIndex(p => p.PublishedDate);
        builder.HasIndex(p => p.IsPublished);
        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.IsPinned);
        builder.HasIndex(p => p.ThemeOfTheYearId);
    }
}