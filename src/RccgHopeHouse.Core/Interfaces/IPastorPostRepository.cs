using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Repository interface for Pastor's Corner posts.
/// Provides data access for the scrolling feed and admin management.
/// </summary>
public interface IPastorPostRepository
{
    /// <summary>
    /// Gets published posts for the public scrolling feed.
    /// Ordered by: Pinned posts first, then by PublishedDate (newest first).
    /// </summary>
    /// <param name="category">Optional category filter</param>
    /// <param name="skip">Number of posts to skip (pagination)</param>
    /// <param name="take">Number of posts to return (page size)</param>
    /// <param name="ct">Cancellation token</param>
    Task<IReadOnlyList<PastorPost>> GetPublishedFeedAsync(
        PostCategory? category = null,
        int skip = 0,
        int take = 10,
        CancellationToken ct = default);

    /// <summary>
    /// Gets a single published post by ID for viewing.
    /// Loads full content and cover image.
    /// </summary>
    Task<PastorPost?> GetPublishedByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Gets the latest published post (most recent).
    /// Used for homepage highlights or "latest message" widgets.
    /// </summary>
    Task<PastorPost?> GetLatestPublishedAsync(CancellationToken ct = default);

    /// <summary>
    /// Gets featured posts for special highlighting.
    /// </summary>
    Task<IReadOnlyList<PastorPost>> GetFeaturedPostsAsync(
        int count = 5,
        CancellationToken ct = default);

    /// <summary>
    /// Gets pinned posts that should appear at the top.
    /// </summary>
    Task<IReadOnlyList<PastorPost>> GetPinnedPostsAsync(CancellationToken ct = default);

    /// <summary>
    /// Gets all published posts (topics/articles) taught under the same year's
    /// theme, ordered by PublishedDate descending. Used to populate the
    /// "other topics under this theme" selector on a post's detail view.
    /// </summary>
    /// <param name="themeOfTheYearId">The theme to fetch sibling posts for.</param>
    /// <param name="excludePostId">Optional post ID to exclude from the results (typically the post currently being viewed).</param>
    /// <param name="ct">Cancellation token</param>
    Task<IReadOnlyList<PastorPost>> GetPublishedByThemeAsync(
        Guid themeOfTheYearId,
        Guid? excludePostId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Gets all posts (including drafts) for admin management.
    /// </summary>
    Task<IReadOnlyList<PastorPost>> GetAllForAdminAsync(
        bool includeDrafts = true,
        CancellationToken ct = default);

    /// <summary>
    /// Gets a single post by ID for admin editing.
    /// </summary>
    Task<PastorPost?> GetByIdForAdminAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Adds a new post to the database.
    /// </summary>
    Task AddAsync(PastorPost post, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing post.
    /// </summary>
    Task UpdateAsync(PastorPost post, CancellationToken ct = default);

    /// <summary>
    /// Deletes a post permanently.
    /// </summary>
    Task DeleteAsync(PastorPost post, CancellationToken ct = default);

    /// <summary>
    /// Gets the total count of published posts.
    /// Used for pagination calculations.
    /// </summary>
    Task<int> GetPublishedCountAsync(
        PostCategory? category = null,
        CancellationToken ct = default);

    /// <summary>
    /// Persists all pending changes.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}