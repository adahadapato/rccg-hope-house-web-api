using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for gallery categories.
/// </summary>
public class GalleryCategoryConfiguration
    : IEntityTypeConfiguration<GalleryCategory>
{
    public void Configure(EntityTypeBuilder<GalleryCategory> builder)
    {
        builder.ToTable("GalleryCategories");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Description)
            .HasMaxLength(500);

        builder.Property(g => g.CoverImageData);

        builder.Property(g => g.DisplayOrder)
            .IsRequired();

        builder.Property(g => g.IsActive)
            .IsRequired();

        builder.HasIndex(g => g.Name)
            .IsUnique();

        builder.HasIndex(g => g.IsActive);

        builder.HasIndex(g => g.DisplayOrder);

        builder.HasMany(g => g.Images)
            .WithOne(i => i.Category)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}