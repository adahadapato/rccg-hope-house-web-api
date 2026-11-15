namespace RccgHopeHouse.Core.Interfaces
{
    /// <summary>
    /// Defines data access operations for SEO metadata associated
    /// with sermons, services, events, devotionals and website pages.
    /// </summary>
    public interface ISeoMetadataRepository
    {
        /// <summary>
        /// Retrieves an SEO metadata record by its unique identifier.
        /// </summary>
        /// <param name="id">The SEO metadata record identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// The matching SEO metadata record, or null if not found.
        /// </returns>
        Task<SeoMetadata?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default);

        /// <summary>
        /// Retrieves SEO metadata associated with an existing content record.
        /// </summary>
        /// <param name="contentType">
        /// The content type, such as Sermon, Service, Event or Devotional.
        /// </param>
        /// <param name="contentId">The identifier of the associated content.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// The matching SEO metadata record, or null if not found.
        /// </returns>
        Task<SeoMetadata?> GetByContentAsync(
            string contentType,
            Guid contentId,
            CancellationToken ct = default);

        /// <summary>
        /// Retrieves SEO metadata associated with a public website path.
        /// </summary>
        /// <param name="pagePath">
        /// The relative website path, such as /sermons/walking-in-faith.
        /// </param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// The matching SEO metadata record, or null if not found.
        /// </returns>
        Task<SeoMetadata?> GetByPagePathAsync(
            string pagePath,
            CancellationToken ct = default);

        /// <summary>
        /// Retrieves active SEO metadata associated with a content record.
        /// Intended for public website requests.
        /// </summary>
        /// <param name="contentType">The associated content type.</param>
        /// <param name="contentId">The associated content identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Active SEO metadata, or null if no active record exists.
        /// </returns>
        Task<SeoMetadata?> GetActiveByContentAsync(
            string contentType,
            Guid contentId,
            CancellationToken ct = default);

        /// <summary>
        /// Retrieves active SEO metadata associated with a website path.
        /// Intended for public website requests.
        /// </summary>
        /// <param name="pagePath">The relative website path.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// Active SEO metadata, or null if no active record exists.
        /// </returns>
        Task<SeoMetadata?> GetActiveByPagePathAsync(
            string pagePath,
            CancellationToken ct = default);

        /// <summary>
        /// Retrieves SEO metadata records using optional content type
        /// and title search filters, with pagination.
        /// Intended for the Smart SEO administration page.
        /// </summary>
        /// <param name="skip">Number of records to skip.</param>
        /// <param name="take">Maximum number of records to return.</param>
        /// <param name="contentType">Optional content type filter.</param>
        /// <param name="search">Optional content title search term.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of matching SEO metadata records.</returns>
        Task<IReadOnlyList<SeoMetadata>> GetPagedAsync(
            int skip,
            int take,
            string? contentType = null,
            string? search = null,
            CancellationToken ct = default);

        /// <summary>
        /// Counts SEO metadata records matching optional filters.
        /// </summary>
        /// <param name="contentType">Optional content type filter.</param>
        /// <param name="search">Optional content title search term.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The number of matching SEO metadata records.</returns>
        Task<int> CountAsync(
            string? contentType = null,
            string? search = null,
            CancellationToken ct = default);

        /// <summary>
        /// Adds a new SEO metadata record to the database context.
        /// </summary>
        /// <param name="metadata">The SEO metadata record to add.</param>
        /// <param name="ct">Cancellation token.</param>
        Task AddAsync(
            SeoMetadata metadata,
            CancellationToken ct = default);

        /// <summary>
        /// Marks an existing SEO metadata record for updating.
        /// </summary>
        /// <param name="metadata">The SEO metadata record to update.</param>
        /// <param name="ct">Cancellation token.</param>
        Task UpdateAsync(
            SeoMetadata metadata,
            CancellationToken ct = default);

        /// <summary>
        /// Marks an SEO metadata record for deletion.
        /// The application's deletion strategy determines whether
        /// the record is soft-deleted or physically removed.
        /// </summary>
        /// <param name="metadata">The SEO metadata record to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        Task DeleteAsync(
            SeoMetadata metadata,
            CancellationToken ct = default);

        /// <summary>
        /// Persists pending SEO metadata changes to the database.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The number of affected database entries.</returns>
        Task<int> SaveChangesAsync(
            CancellationToken ct = default);
    }
}
