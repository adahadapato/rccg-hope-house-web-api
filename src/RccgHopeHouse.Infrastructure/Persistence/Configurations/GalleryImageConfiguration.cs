using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configurations for the Gallery domain: Images, Categories, Tags, and the join entity.
/// </summary>
public class GalleryImageConfiguration : IEntityTypeConfiguration<GalleryImage>
{
    /// <summary>
    /// Configures the <see cref="GalleryImage"/> entity, including binary data storage and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<GalleryImage> builder)
    {
        builder.ToTable("GalleryImages");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Title).IsRequired().HasMaxLength(200);
        builder.Property(g => g.Description).HasMaxLength(1000);
        builder.Property(g => g.AltText).IsRequired().HasMaxLength(255);
        builder.Property(g => g.ContentType).HasMaxLength(50);
        builder.Property(g => g.Photographer).HasMaxLength(100);

        // Full and thumbnail images stored as VARBINARY(MAX)
        builder.Property(g => g.ImageData).IsRequired().HasColumnType("VARBINARY(MAX)");
        builder.Property(g => g.ThumbnailData).HasColumnType("VARBINARY(MAX)");
        builder.Property(g => g.ImageHash).HasMaxLength(64); // SHA256 hex string

        // Indexes for filtering, featured highlights, and event chronology
        builder.HasIndex(g => g.CategoryId);
        builder.HasIndex(g => g.IsFeatured);
        builder.HasIndex(g => g.EventDate);

        // Category relationship: prevent accidental cascade deletes
        builder.HasOne(g => g.Category)
               .WithMany(c => c.Images)
               .HasForeignKey(g => g.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Configuration for <see cref="GalleryCategory"/>.
/// </summary>
public class GalleryCategoryConfiguration : IEntityTypeConfiguration<GalleryCategory>
{
    public void Configure(EntityTypeBuilder<GalleryCategory> builder)
    {
        builder.ToTable("GalleryCategories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.CoverImageData).HasColumnType("VARBINARY(MAX)");
        builder.HasIndex(c => c.IsActive);
        builder.HasIndex(c => c.DisplayOrder);
    }
}

/// <summary>
/// Configuration for <see cref="GalleryTag"/> with unique name constraint.
/// </summary>
public class GalleryTagConfiguration : IEntityTypeConfiguration<GalleryTag>
{
    public void Configure(EntityTypeBuilder<GalleryTag> builder)
    {
        builder.ToTable("GalleryTags");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(50);
        builder.HasIndex(t => t.Name).IsUnique(); // Prevent duplicate tags
    }
}

/// <summary>
/// Configuration for the many-to-many join entity <see cref="GalleryImageTag"/>.
/// </summary>
public class GalleryImageTagConfiguration : IEntityTypeConfiguration<GalleryImageTag>
{
    public void Configure(EntityTypeBuilder<GalleryImageTag> builder)
    {
        builder.ToTable("GalleryImageTags");
        builder.HasKey(it => new { it.ImageId, it.TagId });
        builder.HasIndex(it => it.TagId); // Optimize reverse lookups
    }
}