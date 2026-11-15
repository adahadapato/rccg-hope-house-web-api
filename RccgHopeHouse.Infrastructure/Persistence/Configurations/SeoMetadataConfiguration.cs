
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configures the database mapping for the SeoMetadata entity.
    /// Stores approved SEO metadata for sermons, services, events,
    /// devotionals and standalone website pages in one central table.
    /// </summary>
    public sealed class SeoMetadataConfiguration : IEntityTypeConfiguration<SeoMetadata>
    {
        /// <summary>
        /// Configures the SeoMetadata table, its properties,
        /// relationships and indexes.
        /// </summary>
        /// <param name="builder">
        /// The Entity Framework Core entity type builder.
        /// </param>
        public void Configure(EntityTypeBuilder<SeoMetadata> builder)
        {
            builder.ToTable("SeoMetadata");

            // ==================== Primary Key ====================

            builder.HasKey(x => x.Id);

            // ==================== Content Association ====================

            builder.Property(x => x.ContentType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.ContentId)
                .IsRequired(false);

            builder.Property(x => x.ContentTitle)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.PagePath)
                .IsRequired(false)
                .HasMaxLength(500);

            // ==================== SEO Metadata ====================

            builder.Property(x => x.SeoTitle)
                .IsRequired()
                .HasMaxLength(70);

            builder.Property(x => x.MetaDescription)
                .IsRequired()
                .HasMaxLength(160);

            builder.Property(x => x.Keywords)
                .IsRequired(false)
                .HasMaxLength(500);

            // ==================== Publication Status ====================

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(false);

            // ==================== Indexes ====================

            /*
             * Ensures each existing content record has at most
             * one SEO metadata record for its content type.
             *
             * The same GUID can be used by different content
             * types without creating a conflict.
             */
            builder.HasIndex(x => new { x.ContentType, x.ContentId })
                .IsUnique()
                .HasFilter("[ContentId] IS NOT NULL AND [IsDeleted] = 0")
                .HasDatabaseName("IX_SeoMetadata_ContentType_ContentId");

            /*
             * Ensures each standalone page path has at most
             * one SEO metadata record.
             */
            builder.HasIndex(x => x.PagePath)
                .IsUnique()
                .HasFilter("[PagePath] IS NOT NULL AND [IsDeleted] = 0")
                .HasDatabaseName("IX_SeoMetadata_PagePath");

            /*
             * Supports searching SEO records by their
             * associated content title.
             */
            builder.HasIndex(x => x.ContentTitle)
                .HasDatabaseName("IX_SeoMetadata_ContentTitle");

            /*
             * Supports filtering SEO records by content
             * type and publication status.
             */
            builder.HasIndex(x => new { x.ContentType, x.IsActive })
                .HasDatabaseName("IX_SeoMetadata_ContentType_IsActive");
        }
    }
}
