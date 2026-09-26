using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

public class ServiceBroadcastConfiguration
    : IEntityTypeConfiguration<ServiceBroadcast>
{
    public void Configure(
        EntityTypeBuilder<ServiceBroadcast> builder)
    {
        builder.ToTable(
            "ServiceBroadcasts");

        builder.HasKey(
            broadcast => broadcast.Id);

        builder.Property(
                broadcast =>
                    broadcast.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(
                broadcast =>
                    broadcast.VideoId)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(
                broadcast =>
                    broadcast.Description)
            .HasMaxLength(1000);

        builder.Property(
                broadcast =>
                    broadcast.Theme)
            .HasMaxLength(100);

        builder.Property(
                broadcast =>
                    broadcast.ChurchServiceId)
            .IsRequired();

        builder.Ignore(
            broadcast =>
                broadcast.ThumbnailUrl);

        builder.Ignore(
            broadcast =>
                broadcast.VideoUrl);

        builder.HasIndex(
            broadcast =>
                broadcast.ChurchServiceId);

        builder.HasIndex(
            broadcast => new
            {
                broadcast.ChurchServiceId,
                broadcast.ServiceMonth
            })
            .IsUnique();

        builder.HasIndex(
            broadcast => new
            {
                broadcast.Category,
                broadcast.ServiceMonth
            });
    }
}