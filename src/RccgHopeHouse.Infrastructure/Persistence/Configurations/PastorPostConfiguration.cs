using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="PastorPost"/> entity.
/// Handles rich HTML content, binary cover images, and feed-optimized indexing.
/// </summary>
public class PastorPostConfiguration : IEntityTypeConfiguration<PastorPost>
{
    /// <summary>
    /// Configures the <see cref="PastorPost"/> entity schema, binary storage, and indexes.
    /// </summary>
    /// <param name="builder">The entity type builder for <see cref="PastorPost"/>.</param>
    public void Configure(EntityTypeBuilder<PastorPost> builder)
    {
        builder.ToTable("PastorPosts");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Content).IsRequired(); // HTML content; no arbitrary max length
        builder.Property(p => p.Excerpt).HasMaxLength(500);
        builder.Property(p => p.AuthorName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.BibleReference).HasMaxLength(100);
        builder.Property(p => p.Theme).HasMaxLength(200);

        // Store cover images as VARBINARY(MAX) per architectural decision
        builder.Property(p => p.CoverImageData).HasColumnType("VARBINARY(MAX)");
        builder.Property(p => p.CoverImageContentType).HasMaxLength(50);

        // Indexes for feed queries, admin filtering, and chronological sorting
        builder.HasIndex(p => p.PublishedDate);
        builder.HasIndex(p => p.IsPublished);
        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.IsPinned);
    }
}