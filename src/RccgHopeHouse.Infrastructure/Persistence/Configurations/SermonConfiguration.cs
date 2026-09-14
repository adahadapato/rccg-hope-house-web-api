using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core Fluent API configuration for the <see cref="Sermon"/> entity.
/// Defines table mapping, column constraints, indexes, and relationships.
/// </summary>
public class SermonConfiguration : IEntityTypeConfiguration<Sermon>
{
    /// <summary>
    /// Configures the <see cref="Sermon"/> entity schema and constraints.
    /// </summary>
    /// <param name="builder">The entity type builder for <see cref="Sermon"/>.</param>
    public void Configure(EntityTypeBuilder<Sermon> builder)
    {
        builder.ToTable("Sermons");
        builder.HasKey(s => s.Id);

        // Enforce required fields and reasonable length limits to prevent DB bloat
        builder.Property(s => s.Title).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Speaker).IsRequired().HasMaxLength(100);
        builder.Property(s => s.VideoUrl).IsRequired().HasMaxLength(500);
        builder.Property(s => s.Description).HasMaxLength(2000);

        // Indexes for common query patterns (filtering by date and publication status)
        builder.HasIndex(s => s.ServiceDate);
        builder.HasIndex(s => s.IsPublished);
    }
}