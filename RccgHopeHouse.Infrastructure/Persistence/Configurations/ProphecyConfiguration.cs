using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for individual prophecy statements.
/// </summary>
public class ProphecyConfiguration
    : IEntityTypeConfiguration<Prophecy>
{
    public void Configure(
        EntityTypeBuilder<Prophecy> builder)
    {
        builder.ToTable("Prophecies");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Text)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(p => p.CategoryId)
            .IsRequired();

        builder.Property(p => p.DisplayOrder)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.HasIndex(p => p.CategoryId);

        builder.HasIndex(p => p.DisplayOrder);

        builder.HasIndex(p => p.IsActive);

        /*
         * The Category -> Prophecies relationship is configured
         * in ProphecyCategoryConfiguration.
         */
    }
}