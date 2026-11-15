
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures persistence, relationships and indexes for member celebrations.
///
/// Celebration photographs are managed through the existing Gallery
/// infrastructure. Each celebration may reference a GalleryImage,
/// while PhotoUrl is retained for compatibility with existing records.
/// </summary>
public sealed class CelebrationConfiguration : IEntityTypeConfiguration<Celebration>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Celebration> builder)
    {
        builder.ToTable("Celebrations");

        // ==================== Primary Key ====================
        builder.HasKey(x => x.Id);

        // ==================== Member Relationship ====================
        builder.Property(x => x.MemberId)
            .IsRequired();

        builder.HasOne(x => x.Member)
            .WithMany()
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        // ==================== Existing Gallery Category ====================
        builder.Property(x => x.GalleryCategoryId)
            .IsRequired();

        builder.HasOne(x => x.GalleryCategory)
            .WithMany()
            .HasForeignKey(x => x.GalleryCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // ==================== Celebration Details ====================
        builder.Property(x => x.Occasion)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(x => x.CelebrationDate)
            .IsRequired()
            .HasColumnType("date");

        // ==================== Gallery Image Relationship ====================
        // A celebration can exist without an associated photograph.
        // Photographs are stored and managed by the existing Gallery.
        builder.Property(x => x.GalleryImageId)
            .IsRequired(false);

        builder.HasOne(x => x.GalleryImage)
            .WithMany()
            .HasForeignKey(x => x.GalleryImageId)
            .OnDelete(DeleteBehavior.SetNull);

        // Retain the existing URL/path field for compatibility.
        builder.Property(x => x.PhotoUrl)
            .IsRequired(false)
            .HasMaxLength(2048);

        builder.Property(x => x.PhotoPublicationConsent)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(40);

        // ==================== Workflow Timestamps ====================
        builder.Property(x => x.InvitationSentAt)
            .IsRequired(false);

        builder.Property(x => x.RespondedAt)
            .IsRequired(false);

        builder.Property(x => x.ApprovedByAdminAt)
            .IsRequired(false);

        builder.Property(x => x.PublishedAt)
            .IsRequired(false);

        builder.Property(x => x.WithdrawnAt)
            .IsRequired(false);

        // ==================== Indexes ====================

        // One non-deleted record per member, occasion and occurrence date.
        // A soft-deleted record does not block creating a replacement.
        builder.HasIndex(x => new
        {
            x.MemberId,
            x.Occasion,
            x.CelebrationDate
        })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("IX_Celebrations_Member_Occasion_Date");

        // Supports scheduled discovery of upcoming celebrations.
        builder.HasIndex(x => new { x.CelebrationDate, x.Status })
            .HasDatabaseName("IX_Celebrations_Date_Status");

        // Supports a member's celebration history.
        builder.HasIndex(x => new { x.MemberId, x.CelebrationDate })
            .HasDatabaseName("IX_Celebrations_Member_Date");

        // Supports querying celebrations assigned to a gallery category.
        builder.HasIndex(x => x.GalleryCategoryId)
            .HasDatabaseName("IX_Celebrations_GalleryCategoryId");

        // Supports locating celebrations using a particular gallery image.
        builder.HasIndex(x => x.GalleryImageId)
            .HasDatabaseName("IX_Celebrations_GalleryImageId");
    }
}
