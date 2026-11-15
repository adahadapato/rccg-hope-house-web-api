using System.Text.RegularExpressions;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Entities
{
    public class PastorPost : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public string Content { get; private set; } = string.Empty;
        public string? Excerpt { get; private set; }
        public PostCategory Category { get; private set; }

        // Structured content sections
        public string? IntroHeading { get; private set; }
        public string? IntroText { get; private set; }
        public string? StructuredContentJson { get; private set; }
        public string? ClosingText { get; private set; }

        public byte[]? CoverImageData { get; private set; }
        public string? CoverImageContentType { get; private set; }
        public string AuthorName { get; private set; } = "Pastor";
        public DateTime PublishedDate { get; private set; }
        public bool IsPublished { get; private set; }
        public bool IsPinned { get; private set; }
        public bool IsFeatured { get; private set; }
        public int ViewCount { get; private set; }
        public string? BibleReference { get; private set; }

        /// <summary>
        /// FK to the year's theme this article/topic was taught under.
        /// Required — PastorPost is scoped exclusively to theme-driven article
        /// content (teaching/devotional pieces), not general announcements,
        /// so every post belongs to exactly one year's theme.
        /// </summary>
        public Guid ThemeOfTheYearId { get; private set; }

        /// <summary>
        /// Navigation property to the full theme (title, scripture, etc.)
        /// this post was taught under.
        /// </summary>
        public ThemeOfTheYear ThemeOfTheYear { get; private set; } = null!;

        private PastorPost() { } // EF Core parameterless constructor

        public static PastorPost Create(
            string title,
            string content,
            PostCategory category,
            Guid themeOfTheYearId,
            string authorName = "Pastor",
            DateTime? publishedDate = null,
            string? excerpt = null,
            string? introHeading = null,
            string? introText = null,
            string? structuredContentJson = null,
            string? closingText = null,
            byte[]? coverImageData = null,
            string? coverImageContentType = null,
            string? bibleReference = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

            if (themeOfTheYearId == Guid.Empty)
                throw new ArgumentException("A PastorPost must belong to a ThemeOfTheYear.", nameof(themeOfTheYearId));

            var post = new PastorPost
            {
                Title = title.Trim(),
                Content = content.Trim(),
                Category = category,
                ThemeOfTheYearId = themeOfTheYearId,
                AuthorName = authorName.Trim(),
                PublishedDate = publishedDate ?? default,
                Excerpt = excerpt?.Trim(),
                IntroHeading = introHeading?.Trim(),
                IntroText = introText?.Trim(),
                StructuredContentJson = structuredContentJson,
                ClosingText = closingText?.Trim(),
                CoverImageData = coverImageData,
                CoverImageContentType = coverImageContentType,
                BibleReference = bibleReference?.Trim(),
                IsPublished = false,
                IsPinned = false,
                IsFeatured = false,
                ViewCount = 0
            };

            if (string.IsNullOrWhiteSpace(post.Excerpt))
                post.Excerpt = post.GenerateExcerpt();

            return post;
        }

        public void Publish()
        {
            if (PublishedDate == default)
                PublishedDate = DateTime.UtcNow;

            IsPublished = true;
            MarkAsUpdated();
        }

        public void Unpublish()
        {
            IsPublished = false;
            MarkAsUpdated();
        }

        public void Update(
            string title,
            string content,
            PostCategory category,
            Guid themeOfTheYearId,
            string? excerpt = null,
            string? introHeading = null,
            string? introText = null,
            string? structuredContentJson = null,
            string? closingText = null,
            string? bibleReference = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

            if (themeOfTheYearId == Guid.Empty)
                throw new ArgumentException("A PastorPost must belong to a ThemeOfTheYear.", nameof(themeOfTheYearId));

            Title = title.Trim();
            Content = content.Trim();
            Category = category;
            ThemeOfTheYearId = themeOfTheYearId;
            Excerpt = excerpt?.Trim();
            IntroHeading = introHeading?.Trim();
            IntroText = introText?.Trim();
            StructuredContentJson = structuredContentJson;
            ClosingText = closingText?.Trim();
            BibleReference = bibleReference?.Trim();

            if (string.IsNullOrWhiteSpace(Excerpt))
                Excerpt = GenerateExcerpt();

            MarkAsUpdated();
        }

        public void SetCoverImage(byte[]? imageData, string? contentType)
        {
            CoverImageData = imageData;
            CoverImageContentType = contentType;
            MarkAsUpdated();
        }

        public void Pin() => IsPinned = true;
        public void Unpin() => IsPinned = false;
        public void Feature() => IsFeatured = true;
        public void Unfeature() => IsFeatured = false;
        public void IncrementViewCount() => ViewCount++;

        public string GenerateExcerpt(int maxLength = 150)
        {
            var plainText = Regex.Replace(Content, "<.*?>", string.Empty);

            if (plainText.Length <= maxLength)
                return plainText;

            return plainText.Substring(0, maxLength).TrimEnd() + "...";
        }
    }
}