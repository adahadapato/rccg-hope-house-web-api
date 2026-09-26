using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for ChurchService schedule,
/// presentation, location and Zoom metadata.
/// </summary>
public class ChurchServiceConfiguration
    : IEntityTypeConfiguration<ChurchService>
{
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

        builder.Property(
                service => service.ShowInMonthlyServices)
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