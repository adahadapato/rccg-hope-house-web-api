using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for church events displayed
/// in the public Upcoming Events section.
/// </summary>
public class ChurchEventConfiguration
    : IEntityTypeConfiguration<ChurchEvent>
{
    public void Configure(
        EntityTypeBuilder<ChurchEvent> builder)
    {
        builder.ToTable("ChurchEvents");

        builder.HasKey(
            churchEvent => churchEvent.Id);

        builder.Property(
                churchEvent => churchEvent.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(
                churchEvent => churchEvent.Category)
            .IsRequired();

        builder.Property(
                churchEvent => churchEvent.Description)
            .HasMaxLength(1000);

        builder.Property(
                churchEvent => churchEvent.StartDateTime)
            .IsRequired();

        builder.Property(
            churchEvent => churchEvent.EndDateTime);

        builder.Property(
                churchEvent => churchEvent.Location)
            .HasMaxLength(200);

        builder.Property(
                churchEvent => churchEvent.Icon)
            .HasMaxLength(50);

        builder.Property(
                churchEvent => churchEvent.Color)
            .HasMaxLength(50);

        builder.Property(
                churchEvent => churchEvent.RegistrationUrl)
            .HasMaxLength(1000);

        builder.Property(
                churchEvent =>
                    churchEvent.RegistrationButtonText)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("Register Now");

        builder.Property(
                churchEvent => churchEvent.ImageUrl)
            .HasMaxLength(1000);

        builder.Property(
                churchEvent => churchEvent.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(
                churchEvent => churchEvent.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        /*
         * Supports the public Upcoming Events
         * query:
         *
         * active events ordered/filterable
         * by their scheduled start date.
         */
        builder.HasIndex(
            churchEvent =>
                churchEvent.StartDateTime);

        builder.HasIndex(
            churchEvent =>
                churchEvent.EndDateTime);

        builder.HasIndex(
            churchEvent =>
                churchEvent.Category);

        builder.HasIndex(
            churchEvent =>
                churchEvent.IsActive);

        builder.HasIndex(
            churchEvent =>
                churchEvent.DisplayOrder);

        builder.HasIndex(
            churchEvent => new
            {
                churchEvent.IsActive,
                churchEvent.StartDateTime
            });
    }
}