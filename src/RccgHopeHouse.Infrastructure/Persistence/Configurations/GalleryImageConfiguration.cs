using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for gallery images.
/// Physical image files are stored outside SQL Server.
/// Only file paths and image metadata are persisted here.
/// </summary>
public class GalleryImageConfiguration
    : IEntityTypeConfiguration<GalleryImage>
{
    public void Configure(
        EntityTypeBuilder<GalleryImage> builder)
    {
        builder.ToTable("GalleryImages");

        builder.HasKey(g => g.Id);

        builder.HasQueryFilter(g => !g.IsDeleted);

        builder.Property(g => g.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(g => g.Description)
            .HasMaxLength(1000);

        builder.Property(g => g.ImagePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(g => g.ThumbnailPath)
            .HasMaxLength(500);

        builder.Property(g => g.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.AltText)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(g => g.Photographer)
            .HasMaxLength(100);

        builder.Property(g => g.ImageHash)
            .HasMaxLength(64);

        builder.Property(g => g.FileSizeBytes)
            .IsRequired();

        builder.Property(g => g.Width)
            .IsRequired();

        builder.Property(g => g.Height)
            .IsRequired();

        builder.Property(g => g.DisplayOrder)
            .IsRequired();

        builder.Property(g => g.IsFeatured)
            .IsRequired();

        builder.Property(g => g.IsPublic)
            .IsRequired();

        builder.Property(g => g.ViewCount)
            .IsRequired();

        builder.HasIndex(g => g.CategoryId);

        builder.HasIndex(g => g.IsFeatured);

        builder.HasIndex(g => g.IsPublic);

        builder.HasIndex(g => g.EventDate);

        builder.HasIndex(g => g.DisplayOrder);

        builder.HasIndex(g => g.ImageHash);

        builder.HasOne(g => g.Category)
            .WithMany(c => c.Images)
            .HasForeignKey(g => g.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
