using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for gallery tags.
/// </summary>
public class GalleryTagConfiguration
    : IEntityTypeConfiguration<GalleryTag>
{
    public void Configure(
        EntityTypeBuilder<GalleryTag> builder)
    {
        builder.ToTable("GalleryTags");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(t => t.Name)
            .IsUnique();
    }
}
