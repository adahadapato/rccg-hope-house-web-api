using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for <see cref="ThemeOfTheYear"/> — the annual theme, scripture,
/// and description shown on the homepage's "Theme of the Year" section.
/// </summary>
public class ThemeOfTheYearConfiguration : IEntityTypeConfiguration<ThemeOfTheYear>
{
    public void Configure(EntityTypeBuilder<ThemeOfTheYear> builder)
    {
        builder.ToTable("ThemeOfTheYear");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.ThemeTitle).IsRequired().HasMaxLength(200);
        builder.Property(t => t.ScriptureText).IsRequired().HasMaxLength(1000);
        builder.Property(t => t.ScriptureReference).IsRequired().HasMaxLength(100);
        builder.Property(t => t.PrimaryDescription).IsRequired().HasMaxLength(2000);
        builder.Property(t => t.SecondaryDescription).HasMaxLength(2000);
        builder.Property(t => t.CallToActionText).HasMaxLength(300);

        // Enforces the "one theme per year" business rule at the database level —
        // matches the design decision from when this entity was created: uniqueness
        // is a cross-record rule the entity itself can't enforce, so it belongs here.
        builder.HasIndex(t => t.Year).IsUnique();
    }
}