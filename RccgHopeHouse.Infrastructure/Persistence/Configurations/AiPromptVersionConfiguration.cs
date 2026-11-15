using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for saved versions of editable AI prompts.
/// </summary>
public class AiPromptVersionConfiguration
    : IEntityTypeConfiguration<AiPromptVersion>
{
    public void Configure(
        EntityTypeBuilder<AiPromptVersion> builder)
    {
        builder.ToTable("AiPromptVersions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.PromptKey)
            .IsRequired()
            .HasMaxLength(AiPromptVersion.PromptKeyMaxLength);

        // nvarchar(max): prompts can be long. The 20,000 character
        // limit is enforced by the entity and validator.
        builder.Property(v => v.Instructions)
            .IsRequired();

        builder.Property(v => v.ChangeNote)
            .HasMaxLength(AiPromptVersion.ChangeNoteMaxLength);

        builder.Property(v => v.ChangedBy)
            .HasMaxLength(AiPromptVersion.ChangedByMaxLength);

        builder.Property(v => v.IsActive)
            .IsRequired();

        // One version number per prompt.
        builder.HasIndex(v => new
        {
            v.PromptKey,
            v.Version
        }).IsUnique();

        // Finding the active version quickly.
        builder.HasIndex(v => new
        {
            v.PromptKey,
            v.IsActive
        });
    }
}
