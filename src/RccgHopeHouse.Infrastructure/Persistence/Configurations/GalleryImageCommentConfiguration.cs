using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for birthday wishes left on celebrant photos.
/// </summary>
public class GalleryImageCommentConfiguration
    : IEntityTypeConfiguration<GalleryImageComment>
{
    public void Configure(
        EntityTypeBuilder<GalleryImageComment> builder)
    {
        builder.ToTable("GalleryImageComments");

        builder.HasKey(c => c.Id);

        // Hide wishes for deleted photos, matching the
        // soft-delete filter on GalleryImage.
        builder.HasQueryFilter(c => !c.GalleryImage.IsDeleted);

        builder.Property(c => c.AuthorName)
            .IsRequired()
            .HasMaxLength(GalleryImageComment.AuthorNameMaxLength);

        builder.Property(c => c.Message)
            .IsRequired()
            .HasMaxLength(GalleryImageComment.MessageMaxLength);

        builder.Property(c => c.Status)
            .IsRequired();

        builder.Property(c => c.DecisionSource)
            .IsRequired();

        builder.Property(c => c.DecisionReason)
            .HasMaxLength(GalleryImageComment.DecisionReasonMaxLength);

        builder.Property(c => c.ModeratedBy)
            .HasMaxLength(GalleryImageComment.ModeratedByMaxLength);

        // Deleting a photo removes its wishes.
        builder.HasOne(c => c.GalleryImage)
            .WithMany()
            .HasForeignKey(c => c.GalleryImageId)
            .OnDelete(DeleteBehavior.Cascade);

        // Public list of approved wishes for one photo.
        builder.HasIndex(c => new
        {
            c.GalleryImageId,
            c.Status
        });

        // Admin page tabs, newest first.
        builder.HasIndex(c => new
        {
            c.Status,
            c.CreatedAt
        });
    }
}