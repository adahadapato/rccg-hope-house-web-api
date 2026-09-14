using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for <see cref="PrayerRequest"/>, including Value Object conversion for phone numbers.
/// </summary>
public class PrayerRequestConfiguration : IEntityTypeConfiguration<PrayerRequest>
{
    /// <summary>
    /// Configures the entity schema and applies EF Core value converters for domain Value Objects.
    /// </summary>
    public void Configure(EntityTypeBuilder<PrayerRequest> builder)
    {
        builder.ToTable("PrayerRequests");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.RequesterName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.RequesterEmail).HasMaxLength(255);
        builder.Property(p => p.Content).IsRequired().HasMaxLength(2000);
        builder.Property(p => p.PastoralNote).HasMaxLength(1000);

        /// <remarks>
        /// Converts the <see cref="PhoneNumber"/> Value Object to a simple NVARCHAR column in the database,
        /// and reconstructs the Value Object when materializing entities from the database.
        /// </remarks>
        builder.Property(p => p.PhoneNumber)
               .HasConversion(
                   vo => vo != null ? vo.Value : null,          // Domain → Database
                   value => value != null ? PhoneNumber.CreateOrNull(value) : null); // Database → Domain
    }
}