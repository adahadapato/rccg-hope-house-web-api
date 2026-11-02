using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for the many-to-many relationship
/// between gallery images and tags.
/// </summary>
public class GalleryImageTagConfiguration
    : IEntityTypeConfiguration<GalleryImageTag>
{
    public void Configure(
        EntityTypeBuilder<GalleryImageTag> builder)
    {
        builder.ToTable("GalleryImageTags");

        builder.HasKey(
            it => new
            {
                it.ImageId,
                it.TagId
            });

        // Hide tag links for deleted photos, matching the
        // soft-delete filter on GalleryImage.
        builder.HasQueryFilter(it => !it.Image.IsDeleted);

        builder.HasIndex(it => it.TagId);

        builder.HasOne(it => it.Image)
            .WithMany(i => i.Tags)
            .HasForeignKey(it => it.ImageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(it => it.Tag)
            .WithMany(t => t.Images)
            .HasForeignKey(it => it.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
