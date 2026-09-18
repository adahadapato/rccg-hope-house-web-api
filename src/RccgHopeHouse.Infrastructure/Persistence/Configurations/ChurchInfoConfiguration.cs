using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for <see cref="ChurchInfo"/> — the church's
/// singleton-style profile: address and About-section content.
/// </summary>
public class ChurchInfoConfiguration : IEntityTypeConfiguration<ChurchInfo>
{
    public void Configure(EntityTypeBuilder<ChurchInfo> builder)
    {
        builder.ToTable("ChurchInfo");
        builder.HasKey(c => c.Id);

        // ---- Address ----
        builder.Property(c => c.AddressLine1).IsRequired().HasMaxLength(200);
        builder.Property(c => c.AddressLine2).HasMaxLength(200);
        builder.Property(c => c.City).IsRequired().HasMaxLength(100);
        builder.Property(c => c.PostCode).HasMaxLength(20);
        builder.Property(c => c.Country).IsRequired().HasMaxLength(100);

        // ---- About Section ----
        builder.Property(c => c.ParishName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Tagline).IsRequired().HasMaxLength(300);
        builder.Property(c => c.AboutLead).IsRequired().HasMaxLength(500);
        builder.Property(c => c.AboutText).IsRequired().HasMaxLength(2000);
        builder.Property(c => c.MultiCulturalStat).IsRequired().HasMaxLength(20);

        // YearsOfMinistry is a computed, non-persisted property (derived from
        // EstablishedYear at read time) — explicitly excluded so EF Core
        // doesn't attempt to map it as a column.
        builder.Ignore(c => c.YearsOfMinistry);

        // One-to-many: a church's contact methods. Cascade delete is correct
        // here (unlike PastorPost → ThemeOfTheYear's Restrict) since contact
        // methods have no meaning independent of the ChurchInfo they belong
        // to — deleting the church profile should delete its phone/email
        // entries too.
        builder.HasMany(c => c.ContactMethods)
               .WithOne(m => m.ChurchInfo)
               .HasForeignKey(m => m.ChurchInfoId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}