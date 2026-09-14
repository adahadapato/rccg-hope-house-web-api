namespace RccgHopeHouse.Core.Entities
{
    public class Sermon : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public string Speaker { get; private set; } = string.Empty;
        public DateTime ServiceDate { get; private set; }
        public string VideoUrl { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public bool IsPublished { get; private set; }

        private Sermon() { } // EF Core parameterless constructor


        public static Sermon Create(string title, string speaker, DateTime serviceDate, string videoUrl, string? description = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            ArgumentException.ThrowIfNullOrWhiteSpace(speaker, nameof(speaker));
            ArgumentException.ThrowIfNullOrWhiteSpace(videoUrl, nameof(videoUrl));

            return new Sermon
            {
                Title = title.Trim(),
                Speaker = speaker.Trim(),
                ServiceDate = serviceDate,
                VideoUrl = videoUrl.Trim(),
                Description = description?.Trim(),
                IsPublished = false
            };
        }

        public void Publish() => IsPublished = true;
        public void UpdateDetails(string title, string speaker, DateTime serviceDate, string videoUrl, string? description)
        {
            Title = title;
            Speaker = speaker;
            ServiceDate = serviceDate;
            VideoUrl = videoUrl;
            Description = description;
            MarkAsUpdated();
        }
    }
}
