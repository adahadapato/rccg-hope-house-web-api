using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for <see cref="Offering"/>: financial giving
/// submitted through the Give Online feature.
/// </summary>
public class OfferingConfiguration : IEntityTypeConfiguration<Offering>
{
    public void Configure(EntityTypeBuilder<Offering> builder)
    {
        builder.ToTable("Offerings");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Amount)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(o => o.PaymentStatus)
               .IsRequired();

        builder.Property(o => o.PaymentReference)
               .HasMaxLength(255);

        builder.Property(o => o.FullName)
               .HasMaxLength(200);

        builder.Property(o => o.Email)
               .HasMaxLength(255)
               .HasConversion(
                   vo => vo != null ? vo.Value : null,
                   value => value != null
                       ? EmailAddress.CreateOrNull(value)
                       : null);

        builder.Property(o => o.IsAnonymous)
               .IsRequired();

        builder.Property(o => o.MessageReference)
               .HasMaxLength(500);

        // Every offering must belong to a valid giving type.
        // Restrict deletion so that a giving type referenced by financial
        // records cannot accidentally be removed from the database.
        builder.HasOne(o => o.GivingType)
               .WithMany()
               .HasForeignKey(o => o.GivingTypeId)
               .OnDelete(DeleteBehavior.Restrict);

        // Supports filtering offerings by giving category.
        builder.HasIndex(o => o.GivingTypeId);

        // Supports administrative filtering by payment status.
        builder.HasIndex(o => o.PaymentStatus);

        // Supports displaying the most recent offerings efficiently.
        builder.HasIndex(o => o.CreatedAt);

        // A payment provider reference should identify only one offering.
        // Multiple null values remain permitted because a newly created
        // offering may not yet have a payment reference.
        builder.HasIndex(o => o.PaymentReference)
               .IsUnique()
               .HasFilter("[PaymentReference] IS NOT NULL");
    }
}