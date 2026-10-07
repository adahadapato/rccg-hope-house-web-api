using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for <see cref="ChurchInfo"/> - the church's
/// singleton-style profile containing address, About-section content,
/// and public contact methods.
/// </summary>
public class ChurchInfoConfiguration : IEntityTypeConfiguration<ChurchInfo>
{
    /// <summary>
    /// Configures the database mapping for <see cref="ChurchInfo"/>.
    /// </summary>
    /// <param name="builder">
    /// The entity type builder used to configure the entity.
    /// </param>
    public void Configure(EntityTypeBuilder<ChurchInfo> builder)
    {
        builder.ToTable("ChurchInfo");
        builder.HasKey(c => c.Id);

        // ==================== Address ====================

        builder.Property(c => c.AddressLine1)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.AddressLine2)
            .HasMaxLength(200);

        builder.Property(c => c.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.PostCode)
            .HasMaxLength(20);

        builder.Property(c => c.Country)
            .IsRequired()
            .HasMaxLength(100);

        // ==================== About Section ====================

        builder.Property(c => c.ParishName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Tagline)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(c => c.AboutLead)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.AboutText)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.MultiCulturalStat)
            .IsRequired()
            .HasMaxLength(20);

        // YearsOfMinistry is computed from EstablishedYear at read time
        // and is therefore not persisted in the database.
        builder.Ignore(c => c.YearsOfMinistry);

        // ==================== Contact Methods ====================

        // A ChurchInfo record can have multiple contact methods, including
        // phone, email, website, WhatsApp group, and social media links.
        //
        // Contact methods have no meaning independently of ChurchInfo,
        // so deleting ChurchInfo should also delete its contact methods.
        builder.HasMany(c => c.ContactMethods)
            .WithOne(m => m.ChurchInfo)
            .HasForeignKey(m => m.ChurchInfoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}