using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Entities
{
    public class ServiceBroadcast : BaseEntity
    {
        public ServiceCategory Category { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string VideoId { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public DateTime ServiceMonth { get; private set; }
        public bool IsLive { get; private set; }

        /// <summary>
        /// This month's sermon/service theme badge, e.g. "Divine
        /// Faithfulness". Optional — some broadcasts may not carry a
        /// themed badge every month. Presence of a theme drives whether
        /// the frontend renders it as a "featured" card.
        /// </summary>
        public string? Theme { get; private set; }

        public string ThumbnailUrl => $"https://i.ytimg.com/vi/{VideoId}/hqdefault.jpg";
        public string VideoUrl => $"https://www.youtube.com/watch?v={VideoId}";

        private ServiceBroadcast() { }

        public static ServiceBroadcast Create(
            ServiceCategory category,
            string title,
            string youtubeUrl,
            DateTime serviceMonth,
            string? description = null,
            string? theme = null,
            bool isLive = false)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            var videoId = ExtractVideoId(youtubeUrl);

            return new ServiceBroadcast
            {
                Category = category,
                Title = title.Trim(),
                VideoId = videoId,
                Description = description?.Trim(),
                Theme = theme?.Trim(),
                ServiceMonth = new DateTime(serviceMonth.Year, serviceMonth.Month, 1),
                IsLive = isLive
            };
        }

        public void Update(string title, string youtubeUrl, string? description, string? theme, bool isLive)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            Title = title.Trim();
            VideoId = ExtractVideoId(youtubeUrl);
            Description = description?.Trim();
            Theme = theme?.Trim();
            IsLive = isLive;
            MarkAsUpdated();
        }

        private static string ExtractVideoId(string input)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(input, nameof(input));
            input = input.Trim();

            if (Uri.TryCreate(input, UriKind.Absolute, out var uri))
            {
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                var v = query["v"];
                if (!string.IsNullOrEmpty(v)) return v;

                if (uri.Host.Contains("youtu.be"))
                    return uri.AbsolutePath.Trim('/');
            }

            if (input.Length == 11) return input;

            throw new ArgumentException("Could not extract a YouTube video ID from the given input.", nameof(input));
        }
    }
}