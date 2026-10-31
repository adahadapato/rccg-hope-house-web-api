using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for News &amp; Updates announcements shown
/// in the public floating banner.
/// </summary>
public class AnnouncementConfiguration
    : IEntityTypeConfiguration<Announcement>
{
    public void Configure(
        EntityTypeBuilder<Announcement> builder)
    {
        builder.ToTable("Announcements");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(Announcement.TitleMaxLength);

        builder.Property(a => a.Summary)
            .HasMaxLength(Announcement.SummaryMaxLength);

        builder.Property(a => a.Body)
            .HasMaxLength(Announcement.BodyMaxLength);

        builder.Property(a => a.Type)
            .IsRequired();

        builder.Property(a => a.ImagePath)
            .HasMaxLength(Announcement.ImagePathMaxLength);

        builder.Property(a => a.ThumbnailPath)
            .HasMaxLength(Announcement.ImagePathMaxLength);

        builder.Property(a => a.LinkUrl)
            .HasMaxLength(Announcement.LinkUrlMaxLength);

        builder.Property(a => a.LinkText)
            .HasMaxLength(Announcement.LinkTextMaxLength);

        builder.Property(a => a.VenueName)
            .HasMaxLength(Announcement.VenueNameMaxLength);

        builder.Property(a => a.VenueAddress)
            .HasMaxLength(Announcement.VenueAddressMaxLength);

        builder.Property(a => a.StartsAt)
            .IsRequired();

        builder.Property(a => a.IsPublished)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.ShowInBanner)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // Supports the public "live announcements" query.
        builder.HasIndex(a => new
        {
            a.IsPublished,
            a.StartsAt,
            a.EndsAt
        });

        builder.HasIndex(a => a.DisplayOrder);
    }
}
