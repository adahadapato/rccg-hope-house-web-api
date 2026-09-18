using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for <see cref="PrayerRequest"/>, including Value Object conversion for email and phone.
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
        builder.Property(p => p.Content).IsRequired().HasMaxLength(2000);
        builder.Property(p => p.PastoralNote).HasMaxLength(1000);

        /// <remarks>
        /// RequesterEmail is EmailAddress? (a Value Object), not a plain string —
        /// requires an explicit conversion for EF Core to map it.
        /// </remarks>
        builder.Property(p => p.RequesterEmail)
               .HasMaxLength(255)
               .HasConversion(
                   vo => vo != null ? vo.Value : null,                        // Domain → Database
                   value => value != null ? EmailAddress.CreateOrNull(value) : null); // Database → Domain

        /// <remarks>
        /// FIXED: This conversion was previously missing .HasMaxLength(20), unlike
        /// ContactUsConfiguration's identical PhoneNumber conversion — it would have
        /// generated an unbounded nvarchar(max) column instead of the intended
        /// short, bounded phone number column.
        /// </remarks>
        builder.Property(p => p.PhoneNumber)
               .HasMaxLength(20)
               .HasConversion(
                   vo => vo != null ? vo.Value : null,          // Domain → Database
                   value => value != null ? PhoneNumber.CreateOrNull(value) : null); // Database → Domain
    }
}