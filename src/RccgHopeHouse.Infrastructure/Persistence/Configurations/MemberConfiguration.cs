using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for <see cref="Member"/>: church congregation
/// records used for admin management and automated birthday/anniversary
/// greetings.
/// </summary>
public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(m => m.LastName).IsRequired().HasMaxLength(100);

        builder.Property(m => m.Email)
               .HasMaxLength(255)
               .HasConversion(
                   vo => vo != null ? vo.Value : null,
                   value => value != null ? EmailAddress.CreateOrNull(value) : null);

        builder.Property(m => m.PhoneNumber)
               .HasMaxLength(20)
               .HasConversion(
                   vo => vo != null ? vo.Value : null,
                   value => value != null ? PhoneNumber.CreateOrNull(value) : null);

        // Composite index supports the "who has a birthday today" lookup
        // (WHERE BirthMonth = X AND BirthDay = Y) that a daily background
        // job will run against potentially thousands of members.
        builder.HasIndex(m => new { m.BirthMonth, m.BirthDay });

        builder.HasIndex(m => m.WeddingAnniversary);

        builder.HasIndex(m => m.IsActive);
    }
}