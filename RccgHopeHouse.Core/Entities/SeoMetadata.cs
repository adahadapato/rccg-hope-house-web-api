
namespace RccgHopeHouse.Core.Entities
{
    /// <summary>
    /// Represents approved SEO metadata associated with website content.
    /// Supports sermons, services, events, devotionals and standalone pages
    /// without requiring SEO fields in their respective entities.
    /// </summary>
    public class SeoMetadata : BaseEntity
    {
        /// <summary>
        /// Gets the type of content associated with the SEO metadata.
        /// Examples include Sermon, Service, Event, Devotional and Page.
        /// </summary>
        public string ContentType { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the unique identifier of the associated content record.
        /// Null when the metadata belongs to a standalone website page.
        /// </summary>
        public Guid? ContentId { get; private set; }

        /// <summary>
        /// Gets the original title of the associated website content.
        /// Used for identification and searching within Smart SEO.
        /// </summary>
        public string ContentTitle { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the relative public URL path associated with this metadata.
        /// </summary>
        public string? PagePath { get; private set; }

        /// <summary>
        /// Gets the approved SEO title used in public page metadata.
        /// </summary>
        public string SeoTitle { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the approved meta description for search engines.
        /// </summary>
        public string MetaDescription { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the approved SEO keyword phrases stored as comma-separated text.
        /// </summary>
        public string? Keywords { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this metadata is approved
        /// for use on the public website.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Parameterless constructor required by Entity Framework Core.
        /// </summary>
        private SeoMetadata() { }

        /// <summary>
        /// Creates a new inactive SEO metadata record.
        /// The metadata must be explicitly activated before public use.
        /// </summary>
        /// <param name="contentType">The associated content type.</param>
        /// <param name="contentId">The associated content identifier, if available.</param>
        /// <param name="contentTitle">The title of the associated content.</param>
        /// <param name="pagePath">The relative public page path, if available.</param>
        /// <param name="seoTitle">The approved SEO title.</param>
        /// <param name="metaDescription">The approved meta description.</param>
        /// <param name="keywords">Optional comma-separated keyword phrases.</param>
        /// <returns>A new inactive SEO metadata record.</returns>
        public static SeoMetadata Create(
            string contentType,
            Guid? contentId,
            string contentTitle,
            string? pagePath,
            string seoTitle,
            string metaDescription,
            string? keywords)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
            ArgumentException.ThrowIfNullOrWhiteSpace(contentTitle);

            ValidateSeoMetadata(seoTitle, metaDescription, keywords);

            if (!contentId.HasValue && string.IsNullOrWhiteSpace(pagePath))
                throw new ArgumentException("A content ID or page path is required.");

            return new SeoMetadata
            {
                ContentType = contentType.Trim(),
                ContentId = contentId,
                ContentTitle = contentTitle.Trim(),
                PagePath = string.IsNullOrWhiteSpace(pagePath) ? null : pagePath.Trim(),
                SeoTitle = seoTitle.Trim(),
                MetaDescription = metaDescription.Trim(),
                Keywords = string.IsNullOrWhiteSpace(keywords) ? null : keywords.Trim(),
                IsActive = false
            };
        }

        /// <summary>
        /// Updates the SEO metadata without changing its associated content.
        /// </summary>
        /// <param name="seoTitle">The updated SEO title.</param>
        /// <param name="metaDescription">The updated meta description.</param>
        /// <param name="keywords">The updated keyword phrases.</param>
        public void UpdateMetadata(
            string seoTitle,
            string metaDescription,
            string? keywords)
        {
            ValidateSeoMetadata(seoTitle, metaDescription, keywords);

            SeoTitle = seoTitle.Trim();
            MetaDescription = metaDescription.Trim();
            Keywords = string.IsNullOrWhiteSpace(keywords) ? null : keywords.Trim();

            MarkAsUpdated();
        }

        /// <summary>
        /// Activates the SEO metadata for use on the public website.
        /// </summary>
        public void Activate()
        {
            IsActive = true;
            MarkAsUpdated();
        }

        /// <summary>
        /// Deactivates the SEO metadata without deleting the record.
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            MarkAsUpdated();
        }

        /// <summary>
        /// Validates SEO metadata before creating or updating a record.
        /// </summary>
        private static void ValidateSeoMetadata(
            string seoTitle,
            string metaDescription,
            string? keywords)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(seoTitle);
            ArgumentException.ThrowIfNullOrWhiteSpace(metaDescription);

            if (seoTitle.Trim().Length > 70)
                throw new ArgumentException("SEO title cannot exceed 70 characters.", nameof(seoTitle));

            if (metaDescription.Trim().Length > 160)
                throw new ArgumentException("Meta description cannot exceed 160 characters.", nameof(metaDescription));

            if (keywords?.Trim().Length > 500)
                throw new ArgumentException("SEO keywords cannot exceed 500 characters.", nameof(keywords));
        }
    }
}
