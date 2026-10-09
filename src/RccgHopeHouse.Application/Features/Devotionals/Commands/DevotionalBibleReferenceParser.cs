
using System.Text.RegularExpressions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Devotionals.Commands;

/// <summary>
/// Validates Bible references and generates API.Bible passage IDs
/// using the application's existing Bible reference catalogue.
/// </summary>
internal static class DevotionalBibleReferenceParser
{
    private static readonly Regex ReferencePattern = new(
        @"^\s*(?<book>(?:[1-3]\s*)?[A-Za-z][A-Za-z\s]*)\s+" +
        @"(?<chapter>\d+)" +
        @"(?::(?<start>\d+)(?:\s*[-–—]\s*(?<end>\d+))?)?\s*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Converts a single verse, verse range or full chapter
    /// reference into a validated passage ID.
    /// </summary>
    public static string BuildPassageId(
        string reference,
        IBibleReferenceService bibleReferenceService)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reference);
        ArgumentNullException.ThrowIfNull(bibleReferenceService);

        var match = ReferencePattern.Match(reference);

        if (!match.Success)
        {
            throw new ArgumentException(
                $"Invalid Bible reference: '{reference}'.");
        }

        var bookName = Normalize(match.Groups["book"].Value);

        var book = bibleReferenceService.GetBooks()
            .FirstOrDefault(candidate =>
                Normalize(candidate.Name) == bookName ||
                Normalize(candidate.Code) == bookName);

        if (book is null)
        {
            throw new ArgumentException(
                $"Unknown Bible book: '{reference}'.");
        }

        var chapter = int.Parse(match.Groups["chapter"].Value);

        if (chapter < 1 ||
            chapter > book.ChapterVerseCounts.Count)
        {
            throw new ArgumentException(
                $"Invalid chapter in '{reference}'.");
        }

        var hasVerse = match.Groups["start"].Success;

        if (!hasVerse)
        {
            return $"{book.Code}.{chapter}";
        }

        var startVerse = int.Parse(
            match.Groups["start"].Value);

        var endVerse = match.Groups["end"].Success
            ? int.Parse(match.Groups["end"].Value)
            : startVerse;

        if (!bibleReferenceService.IsValidReference(
                book.Code,
                chapter,
                startVerse,
                endVerse))
        {
            throw new ArgumentException(
                $"Invalid verse range in '{reference}'.");
        }

        return bibleReferenceService.BuildPassageId(
            book.Code,
            chapter,
            startVerse,
            endVerse);
    }

    /// <summary>
    /// Converts selected annual Bible readings into passage IDs.
    /// A chapter range is expanded into individual chapters.
    /// </summary>
    public static IReadOnlyList<string> BuildPassageIds(
        IEnumerable<string> references,
        IBibleReferenceService bibleReferenceService)
    {
        ArgumentNullException.ThrowIfNull(references);

        var result = new List<string>();

        foreach (var reference in references)
        {
            if (string.IsNullOrWhiteSpace(reference))
                continue;

            var rangeMatch = Regex.Match(
                reference,
                @"^\s*(?<book>(?:[1-3]\s*)?[A-Za-z][A-Za-z\s]*)\s+" +
                @"(?<start>\d+)\s*[-–—]\s*(?<end>\d+)\s*$",
                RegexOptions.IgnoreCase);

            if (rangeMatch.Success)
            {
                var bookName = rangeMatch.Groups["book"].Value;
                var start = int.Parse(rangeMatch.Groups["start"].Value);
                var end = int.Parse(rangeMatch.Groups["end"].Value);

                if (end < start || end - start > 149)
                {
                    throw new ArgumentException(
                        $"Invalid chapter range: '{reference}'.");
                }

                for (var chapter = start; chapter <= end; chapter++)
                {
                    result.Add(BuildPassageId(
                        $"{bookName} {chapter}",
                        bibleReferenceService));
                }
            }
            else
            {
                result.Add(BuildPassageId(
                    reference,
                    bibleReferenceService));
            }
        }

        return result
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string Normalize(string value) =>
        Regex.Replace(value, @"\s+", "")
            .Trim()
            .ToUpperInvariant();
}
