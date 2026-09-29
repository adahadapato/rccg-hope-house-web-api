using System.Text.Json;

namespace RccgHopeHouse.Core.Entities
{
    public class Devotional : BaseEntity
    {
        public DateOnly DevotionalDate { get; private set; }

        public string Theme { get; private set; } = string.Empty;

        public string ScriptureReference { get; private set; } = string.Empty;

        /// <summary>
        /// API.Bible/USFM passage identifier used to retrieve
        /// the scripture text, for example:
        /// EXO.14.1-EXO.14.4
        /// </summary>
        public string PassageId { get; private set; } = string.Empty;

        public string Thought { get; private set; } = string.Empty;

        /// <summary>
        /// Ordered commentary points stored as JSON.
        /// </summary>
        public string CommentaryJson { get; private set; } = "[]";

        /// <summary>
        /// Ordered prayer points stored as JSON.
        /// </summary>
        public string PrayerPointsJson { get; private set; } = "[]";

        public string Declaration { get; private set; } = string.Empty;

        public bool IsPublished { get; private set; }

        public DateTime? PublishedAt { get; private set; }


        private Devotional()
        {
        }


        public static Devotional Create(DateOnly devotionalDate, string theme, string scriptureReference,
            string passageId, string thought, IEnumerable<string> commentaryPoints, IEnumerable<string> prayerPoints,
            string declaration)
        {
            Validate(devotionalDate, theme, scriptureReference,
                passageId,
                thought,
                commentaryPoints,
                prayerPoints,
                declaration);

            var cleanedCommentaryPoints =
                CleanCommentaryPoints(commentaryPoints);

            var cleanedPrayerPoints =
                CleanPrayerPoints(prayerPoints);

            return new Devotional
            {
                DevotionalDate = devotionalDate,
                Theme = theme.Trim(),
                ScriptureReference = scriptureReference.Trim(),
                PassageId = passageId.Trim(),
                Thought = thought.Trim(),
                CommentaryJson =
                    JsonSerializer.Serialize(cleanedCommentaryPoints),
                PrayerPointsJson =
                    JsonSerializer.Serialize(cleanedPrayerPoints),
                Declaration = declaration.Trim(),
                IsPublished = false,
                PublishedAt = null
            };
        }


        public void Update(
            DateOnly devotionalDate,
            string theme,
            string scriptureReference,
            string passageId,
            string thought,
            IEnumerable<string> commentaryPoints,
            IEnumerable<string> prayerPoints,
            string declaration)
        {
            Validate(
                devotionalDate,
                theme,
                scriptureReference,
                passageId,
                thought,
                commentaryPoints,
                prayerPoints,
                declaration);

            var cleanedCommentaryPoints =
                CleanCommentaryPoints(commentaryPoints);

            var cleanedPrayerPoints =
                CleanPrayerPoints(prayerPoints);

            DevotionalDate = devotionalDate;
            Theme = theme.Trim();
            ScriptureReference = scriptureReference.Trim();
            PassageId = passageId.Trim();
            Thought = thought.Trim();
            CommentaryJson = JsonSerializer.Serialize(cleanedCommentaryPoints);
            PrayerPointsJson = JsonSerializer.Serialize(cleanedPrayerPoints);
            Declaration = declaration.Trim();

            MarkAsUpdated();
        }


        public void Publish()
        {
            if (IsPublished)
                return;

            IsPublished = true;
            PublishedAt = DateTime.UtcNow;

            MarkAsUpdated();
        }


        public void Unpublish()
        {
            if (!IsPublished)
                return;

            IsPublished = false;

            MarkAsUpdated();
        }


        public IReadOnlyList<string> GetCommentaryPoints()
        {
            if (string.IsNullOrWhiteSpace(CommentaryJson))
                return Array.Empty<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(CommentaryJson)  ?? [];
            }
            catch (JsonException)
            {
                return Array.Empty<string>();
            }
        }


        public IReadOnlyList<string> GetPrayerPoints()
        {
            if (string.IsNullOrWhiteSpace(PrayerPointsJson))
                return Array.Empty<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(PrayerPointsJson) ?? [];
            }
            catch (JsonException)
            {
                return Array.Empty<string>();
            }
        }


        private static List<string> CleanCommentaryPoints(
            IEnumerable<string> commentaryPoints)
        {
            ArgumentNullException.ThrowIfNull(commentaryPoints);

            var cleaned = commentaryPoints
                .Where(point =>
                    !string.IsNullOrWhiteSpace(point))
                .Select(point =>
                    point.Trim())
                .ToList();

            if (cleaned.Count == 0)
            {
                throw new ArgumentException(
                    "At least one commentary point is required.",
                    nameof(commentaryPoints));
            }

            return cleaned;
        }


        private static List<string> CleanPrayerPoints(
            IEnumerable<string> prayerPoints)
        {
            ArgumentNullException.ThrowIfNull(prayerPoints);

            var cleaned = prayerPoints
                .Where(point =>
                    !string.IsNullOrWhiteSpace(point))
                .Select(point =>
                    point.Trim())
                .ToList();

            if (cleaned.Count == 0)
            {
                throw new ArgumentException(
                    "At least one prayer point is required.",
                    nameof(prayerPoints));
            }

            return cleaned;
        }


        private static void Validate(
            DateOnly devotionalDate,
            string theme,
            string scriptureReference,
            string passageId,
            string thought,
            IEnumerable<string> commentaryPoints,
            IEnumerable<string> prayerPoints,
            string declaration)
        {
            if (devotionalDate == default)
            {
                throw new ArgumentException(
                    "Devotional date is required.",
                    nameof(devotionalDate));
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(
                theme,
                nameof(theme));

            ArgumentException.ThrowIfNullOrWhiteSpace(
                scriptureReference,
                nameof(scriptureReference));

            ArgumentException.ThrowIfNullOrWhiteSpace(
                passageId,
                nameof(passageId));

            ArgumentException.ThrowIfNullOrWhiteSpace(
                thought,
                nameof(thought));

            ArgumentException.ThrowIfNullOrWhiteSpace(
                declaration,
                nameof(declaration));

            _ = CleanCommentaryPoints(commentaryPoints);
            _ = CleanPrayerPoints(prayerPoints);
        }
    }
}