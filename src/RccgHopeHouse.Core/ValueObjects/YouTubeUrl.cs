using System.Text.RegularExpressions;

namespace RccgHopeHouse.Core.ValueObjects
{
    public partial record YouTubeUrl(string Value)
    {
        [GeneratedRegex(@"^(https?://)?(www\.)?(youtube\.com|youtu\.be)/.+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
        private static partial Regex ValidPattern();

        public static YouTubeUrl Create(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("YouTube URL cannot be empty.", nameof(url));

            if (!ValidPattern().IsMatch(url))
                throw new ArgumentException("Invalid YouTube URL format.", nameof(url));

            return new YouTubeUrl(url);
        }
    }
}
