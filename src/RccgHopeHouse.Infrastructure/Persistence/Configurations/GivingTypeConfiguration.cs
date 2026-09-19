using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for <see cref="GivingType"/>: categories used
/// to classify financial giving such as Tithe, Offering, Seed Offering,
/// Building Fund and Welfare.
/// </summary>
public class GivingTypeConfiguration : IEntityTypeConfiguration<GivingType>
{
    public void Configure(EntityTypeBuilder<GivingType> builder)
    {
        builder.ToTable("GivingTypes");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(g => g.Description)
               .HasMaxLength(500);

        builder.Property(g => g.IsActive)
               .IsRequired();

        builder.Property(g => g.DisplayOrder)
               .IsRequired();

        // Giving type names must be unique so that duplicate categories
        // such as multiple "Tithe" or "Offering" records cannot be created.
        builder.HasIndex(g => g.Name)
               .IsUnique();

        // Supports efficient retrieval of active giving types for the
        // public Give Online form.
        builder.HasIndex(g => g.IsActive);

        // Supports ordering giving types for display on the website
        // and within administrative interfaces.
        builder.HasIndex(g => g.DisplayOrder);
    }
}