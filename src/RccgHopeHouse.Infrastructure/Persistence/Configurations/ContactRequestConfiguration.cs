using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for <see cref="ContactUs"/> form submissions.
/// </summary>
public class ContactUsConfiguration : IEntityTypeConfiguration<ContactUs>
{
    public void Configure(EntityTypeBuilder<ContactUs> builder)
    {
        builder.ToTable("ContactUs");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Message).IsRequired().HasMaxLength(2000);

        /// <remarks>
        /// FIXED: Email and PhoneNumber are Value Objects (EmailAddress, PhoneNumber),
        /// not plain strings — EF Core cannot map them without an explicit conversion.
        /// This was previously missing entirely, which would throw at model-build time
        /// the first time the app actually touched the database.
        /// </remarks>
        builder.Property(c => c.Email)
               .IsRequired()
               .HasMaxLength(255)
               .HasConversion(
                   vo => vo.Value,                                  // Domain → Database
                   value => EmailAddress.Create(value));             // Database → Domain

        builder.Property(c => c.PhoneNumber)
               .HasMaxLength(20)
               .HasConversion(
                   vo => vo != null ? vo.Value : null,               // Domain → Database
                   value => value != null ? PhoneNumber.CreateOrNull(value) : null); // Database → Domain

        // Indexes for admin inbox filtering and chronological views
        builder.HasIndex(c => c.IsRead);
        builder.HasIndex(c => c.CreatedAt);
        builder.HasIndex(c => c.Reason);
    }
}