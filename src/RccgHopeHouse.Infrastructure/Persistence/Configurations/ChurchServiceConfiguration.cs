using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for ChurchService schedule,
/// presentation, location, Zoom metadata,
/// monthly-service visibility, broadcast settings
/// and current service theme.
/// </summary>
public sealed class ChurchServiceConfiguration
    : IEntityTypeConfiguration<ChurchService>
{
    /// <summary>
    /// Configures the database mapping, constraints,
    /// indexes and relationships for
    /// <see cref="ChurchService"/>.
    /// </summary>
    /// <param name="builder">
    /// Entity Framework configuration builder
    /// for the ChurchService entity.
    /// </param>
    public void Configure(
        EntityTypeBuilder<ChurchService> builder)
    {
        builder.ToTable("ChurchServices");

        builder.HasKey(
            service => service.Id);

        builder.Property(
                service => service.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(
                service => service.Description)
            .HasMaxLength(500);

        builder.Property(
                service => service.Location)
            .HasMaxLength(100);

        builder.Property(
                service => service.ZoomId)
            .HasMaxLength(50);

        builder.Property(
                service => service.ZoomPasscode)
            .HasMaxLength(20);

        builder.Property(
                service => service.AdditionalInfo)
            .HasMaxLength(1000);

        builder.Property(
                service => service.Icon)
            .HasMaxLength(50);

        /// <summary>
        /// Stores the theme for the upcoming or currently
        /// occurring instance of the church service.
        ///
        /// The value is optional because not every church
        /// service will necessarily have a published theme.
        ///
        /// Historical themes belonging to previous broadcasts
        /// remain stored on their corresponding
        /// ServiceBroadcast records.
        /// </summary>
        builder.Property(
                service => service.CurrentTheme)
            .HasMaxLength(200);

        /// <summary>
        /// Controls whether the service is displayed
        /// in the public Monthly Services section.
        /// </summary>
        builder.Property(
                service => service.ShowInMonthlyServices)
            .IsRequired()
            .HasDefaultValue(false);

        /// <summary>
        /// Controls whether broadcasts associated with
        /// this church service are eligible to appear
        /// in the public service broadcast feed.
        ///
        /// Defaults to false so existing and newly created
        /// services are not automatically broadcast-enabled.
        /// </summary>
        builder.Property(
                service => service.IsBroadcastEnabled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(
            service => service.DayOfWeek);

        builder.HasIndex(
            service => service.IsActive);

        builder.HasIndex(
            service => service.DisplayOrder);

        builder.HasIndex(
            service => service.IsLocal);

        builder.HasIndex(
            service =>
                service.ShowInMonthlyServices);

        /// <summary>
        /// Supports efficient filtering of church services
        /// when retrieving public broadcast content.
        /// </summary>
        builder.HasIndex(
            service =>
                service.IsBroadcastEnabled);

        /// <summary>
        /// A church service can have multiple historical
        /// broadcasts, while each ServiceBroadcast belongs
        /// to one specific ChurchService.
        ///
        /// ChurchServiceId is the authoritative relationship
        /// between a broadcast and its service.
        /// </summary>
        builder.HasMany(
                service =>
                    service.Broadcasts)
            .WithOne(
                broadcast =>
                    broadcast.ChurchService)
            .HasForeignKey(
                broadcast =>
                    broadcast.ChurchServiceId)
            .OnDelete(
                DeleteBehavior.Restrict);
    }
}