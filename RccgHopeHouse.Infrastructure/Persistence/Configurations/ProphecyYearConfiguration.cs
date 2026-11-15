using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for prophecy years.
/// </summary>
public class ProphecyYearConfiguration
    : IEntityTypeConfiguration<ProphecyYear>
{
    public void Configure(
        EntityTypeBuilder<ProphecyYear> builder)
    {
        builder.ToTable("ProphecyYears");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Year)
            .IsRequired();

        builder.Property(p => p.IsPublished)
            .IsRequired();

        builder.HasIndex(p => p.Year)
            .IsUnique();

        builder.HasIndex(p => p.IsPublished);

        /*
         * A prophecy category belongs to a prophecy year.
         *
         * Categories have no independent meaning once their
         * parent prophecy year is deleted, so they are deleted
         * together with the year.
         */
        builder.HasMany(p => p.Categories)
            .WithOne(c => c.ProphecyYear)
            .HasForeignKey(c => c.ProphecyYearId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}