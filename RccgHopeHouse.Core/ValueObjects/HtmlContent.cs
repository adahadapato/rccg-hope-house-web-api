using System.Text.RegularExpressions;

namespace RccgHopeHouse.Core.ValueObjects;

/// <summary>
/// Value object for sanitized HTML content.
/// Ensures content is trimmed and optionally validates against XSS patterns.
/// Use for PastorPost.Content, GalleryImage.Description, etc.
/// </summary>
public partial record HtmlContent(string Value)
{
    /// <summary>
    /// Basic XSS pattern to flag suspicious scripts (not a full sanitizer).
    /// For production, use a library like HtmlSanitizer.
    /// </summary>
    [GeneratedRegex(@"<script[^>]*>[\s\S]*?</script>", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex ScriptTagPattern();

    /// <summary>
    /// Creates a new HtmlContent instance with validation.
    /// </summary>
    /// <param name="html">Raw HTML string</param>
    /// <param name="allowScripts">If false, rejects content containing &lt;script&gt; tags</param>
    /// <returns>Validated HtmlContent</returns>
    /// <exception cref="ArgumentException">Thrown when content is empty or contains disallowed scripts</exception>
    public static HtmlContent Create(string html, bool allowScripts = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(html, nameof(html));

        var trimmed = html.Trim();

        if (!allowScripts && ScriptTagPattern().IsMatch(trimmed))
            throw new ArgumentException("Content contains disallowed script tags.", nameof(html));

        return new HtmlContent(trimmed);
    }

    /// <summary>
    /// Returns plain text excerpt by stripping HTML tags.
    /// </summary>
    public string ToPlainText(int? maxLength = null)
    {
        var plain = Regex.Replace(Value, "<.*?>", string.Empty);
        return maxLength.HasValue && plain.Length > maxLength
            ? plain.Substring(0, maxLength.Value).TrimEnd() + "..."
            : plain;
    }

    /// <summary>
    /// Returns the raw HTML value (use with caution in Razor views).
    /// </summary>
    public override string ToString() => Value;
}