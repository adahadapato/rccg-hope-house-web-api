using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for prophecy categories.
/// </summary>
public class ProphecyCategoryConfiguration
    : IEntityTypeConfiguration<ProphecyCategory>
{
    public void Configure(
        EntityTypeBuilder<ProphecyCategory> builder)
    {
        builder.ToTable("ProphecyCategories");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.DisplayOrder)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.ProphecyYearId)
            .IsRequired();

        builder.HasIndex(p => p.ProphecyYearId);

        builder.HasIndex(p => p.DisplayOrder);

        builder.HasIndex(p => p.IsActive);

        /*
         * Category names are unique within a prophecy year.
         * The same category name may therefore be reused
         * in different years.
         *
         * Example:
         * 2026 -> General Prophecies
         * 2027 -> General Prophecies
         */
        builder.HasIndex(
                p => new
                {
                    p.ProphecyYearId,
                    p.Name
                })
            .IsUnique();

        /*
         * The ProphecyYear -> Categories relationship is configured
         * in ProphecyYearConfiguration.
         *
         * A prophecy statement has no independent meaning once its
         * parent category is deleted, so it is deleted together with
         * the category.
         */
        builder.HasMany(p => p.Prophecies)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}