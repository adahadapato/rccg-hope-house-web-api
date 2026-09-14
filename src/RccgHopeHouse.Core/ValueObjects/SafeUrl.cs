namespace RccgHopeHouse.Core.ValueObjects;

/// <summary>
/// Value object for validated, safe URLs.
/// Ensures URLs are well-formed and use allowed schemes (https, http).
/// Use for VideoUrl, ImageUrl, YouTube links, etc.
/// </summary>
public record SafeUrl(string Value)
{
    /// <summary>
    /// Allowed URL schemes for security.
    /// </summary>
    private static readonly HashSet<string> AllowedSchemes = new(StringComparer.OrdinalIgnoreCase)
    {
        "https", "http"
        // Add "mailto:", "tel:" if needed for contact links
    };

    /// <summary>
    /// Creates a new SafeUrl with validation.
    /// </summary>
    /// <param name="url">Raw URL string</param>
    /// <param name="requireHttps">If true, rejects non-https URLs</param>
    /// <returns>Validated SafeUrl</returns>
    /// <exception cref="ArgumentException">Thrown when URL is invalid or uses disallowed scheme</exception>
    public static SafeUrl Create(string url, bool requireHttps = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url, nameof(url));

        var trimmed = url.Trim();

        if (!Uri.IsWellFormedUriString(trimmed, UriKind.Absolute))
            throw new ArgumentException("URL is not well-formed.", nameof(url));

        var uri = new Uri(trimmed, UriKind.Absolute);

        if (!AllowedSchemes.Contains(uri.Scheme))
            throw new ArgumentException($"URL scheme '{uri.Scheme}' is not allowed.", nameof(url));

        if (requireHttps && !uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("HTTPS is required for this URL.", nameof(url));

        return new SafeUrl(trimmed);
    }

    /// <summary>
    /// Returns the URL as a string.
    /// </summary>
    public override string ToString() => Value;
}