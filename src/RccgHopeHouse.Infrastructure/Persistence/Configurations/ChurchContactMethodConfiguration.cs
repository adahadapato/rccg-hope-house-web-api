using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for <see cref="ChurchContactMethod"/> - individual
/// contact, website, messaging, and social media methods belonging to
/// a <see cref="ChurchInfo"/> record.
/// </summary>
public class ChurchContactMethodConfiguration
    : IEntityTypeConfiguration<ChurchContactMethod>
{
    /// <summary>
    /// Configures the database mapping for
    /// <see cref="ChurchContactMethod"/>.
    /// </summary>
    /// <param name="builder">
    /// The entity type builder used to configure the entity.
    /// </param>
    public void Configure(
        EntityTypeBuilder<ChurchContactMethod> builder)
    {
        builder.ToTable("ChurchContactMethods");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Type)
            .IsRequired();

        builder.Property(m => m.Value)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(m => m.Label)
            .HasMaxLength(100);

        builder.Property(m => m.DisplayOrder)
            .IsRequired();

        builder.HasIndex(m => m.ChurchInfoId);

        builder.HasIndex(m => m.DisplayOrder);
    }
}