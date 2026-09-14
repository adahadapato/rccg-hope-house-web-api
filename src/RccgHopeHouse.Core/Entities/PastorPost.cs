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

        // Structured content sections (Option B)
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
        public string? Theme { get; private set; }

        private PastorPost() { } // EF Core parameterless constructor

        public static PastorPost Create(
            string title,
            string content,
            PostCategory category,
            string authorName = "Pastor",
            DateTime? publishedDate = null,
            string? excerpt = null,
            string? introHeading = null,
            string? introText = null,
            string? structuredContentJson = null,
            string? closingText = null,
            byte[]? coverImageData = null,
            string? coverImageContentType = null,
            string? bibleReference = null,
            string? theme = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

            var post = new PastorPost
            {
                Title = title.Trim(),
                Content = content.Trim(),
                Category = category,
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
                Theme = theme?.Trim(),
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
            string? excerpt = null,
            string? introHeading = null,
            string? introText = null,
            string? structuredContentJson = null,
            string? closingText = null,
            string? bibleReference = null,
            string? theme = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

            Title = title.Trim();
            Content = content.Trim();
            Category = category;
            Excerpt = excerpt?.Trim();
            IntroHeading = introHeading?.Trim();
            IntroText = introText?.Trim();
            StructuredContentJson = structuredContentJson;
            ClosingText = closingText?.Trim();
            BibleReference = bibleReference?.Trim();
            Theme = theme?.Trim();

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