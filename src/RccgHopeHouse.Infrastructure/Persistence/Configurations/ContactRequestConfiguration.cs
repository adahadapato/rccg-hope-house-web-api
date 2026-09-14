using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for <see cref="ContactRequest"/> form submissions.
/// </summary>
public class ContactUsConfiguration : IEntityTypeConfiguration<ContactUs>
{
    public void Configure(EntityTypeBuilder<ContactUs> builder)
    {
        builder.ToTable("ContactUs");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(255);
        builder.Property(c => c.PhoneNumber).HasMaxLength(20);
        builder.Property(c => c.Message).IsRequired().HasMaxLength(2000);

        // Indexes for admin inbox filtering and chronological views
        builder.HasIndex(c => c.IsRead);
        builder.HasIndex(c => c.CreatedAt);
        builder.HasIndex(c => c.Reason);
    }
}