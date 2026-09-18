using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for <see cref="ChurchContactMethod"/> — individual
/// phone/email entries belonging to a ChurchInfo.
/// </summary>
public class ChurchContactMethodConfiguration : IEntityTypeConfiguration<ChurchContactMethod>
{
    public void Configure(EntityTypeBuilder<ChurchContactMethod> builder)
    {
        builder.ToTable("ChurchContactMethods");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Type).IsRequired();
        builder.Property(m => m.Value).IsRequired().HasMaxLength(255);
        builder.Property(m => m.Label).HasMaxLength(100);

        builder.HasIndex(m => m.ChurchInfoId);
        builder.HasIndex(m => m.DisplayOrder);
    }
}