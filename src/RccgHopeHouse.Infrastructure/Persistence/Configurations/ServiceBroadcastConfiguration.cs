using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

public class ServiceBroadcastConfiguration : IEntityTypeConfiguration<ServiceBroadcast>
{
    public void Configure(EntityTypeBuilder<ServiceBroadcast> builder)
    {
        builder.ToTable("ServiceBroadcasts");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title).IsRequired().HasMaxLength(200);
        builder.Property(b => b.VideoId).IsRequired().HasMaxLength(20);
        builder.Property(b => b.Description).HasMaxLength(1000);
        builder.Property(b => b.Theme).HasMaxLength(100);

        builder.Ignore(b => b.ThumbnailUrl);
        builder.Ignore(b => b.VideoUrl);

        builder.HasIndex(b => new { b.Category, b.ServiceMonth });
    }
}