namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents the annual prayer content displayed
/// on the public website.
/// </summary>
public class AnnualPrayer : BaseEntity
{
    public int Year { get; private set; }

    public string Theme { get; private set; } =
        string.Empty;

    public string Service { get; private set; } =
        string.Empty;

    public string Author { get; private set; } =
        string.Empty;

    public string BibleReference { get; private set; } =
        string.Empty;

    public string BibleText { get; private set; } =
        string.Empty;

    public string Declaration { get; private set; } =
        string.Empty;

    public string ClosingVerse { get; private set; } =
        string.Empty;

    public string? ImageUrl { get; private set; }

    public bool IsActive { get; private set; }

    public ICollection<AnnualPrayerPoint> PrayerPoints
    {
        get;
        private set;
    } = new List<AnnualPrayerPoint>();

    private AnnualPrayer()
    {
    }

    public static AnnualPrayer Create(
        int year,
        string theme,
        string service,
        string author,
        string bibleReference,
        string bibleText,
        string declaration,
        string closingVerse,
        string? imageUrl = null,
        bool isActive = true)
    {
        ValidateYear(year);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            theme,
            nameof(theme));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            service,
            nameof(service));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            author,
            nameof(author));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            bibleReference,
            nameof(bibleReference));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            bibleText,
            nameof(bibleText));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            declaration,
            nameof(declaration));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            closingVerse,
            nameof(closingVerse));

        return new AnnualPrayer
        {
            Year = year,
            Theme = theme.Trim(),
            Service = service.Trim(),
            Author = author.Trim(),
            BibleReference =
                bibleReference.Trim(),
            BibleText = bibleText.Trim(),
            Declaration =
                declaration.Trim(),
            ClosingVerse =
                closingVerse.Trim(),
            ImageUrl =
                NormalizeOptionalText(
                    imageUrl),
            IsActive = isActive
        };
    }

    public void Update(
        int year,
        string theme,
        string service,
        string author,
        string bibleReference,
        string bibleText,
        string declaration,
        string closingVerse,
        string? imageUrl,
        bool isActive)
    {
        ValidateYear(year);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            theme,
            nameof(theme));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            service,
            nameof(service));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            author,
            nameof(author));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            bibleReference,
            nameof(bibleReference));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            bibleText,
            nameof(bibleText));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            declaration,
            nameof(declaration));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            closingVerse,
            nameof(closingVerse));

        Year = year;
        Theme = theme.Trim();
        Service = service.Trim();
        Author = author.Trim();
        BibleReference =
            bibleReference.Trim();
        BibleText = bibleText.Trim();
        Declaration =
            declaration.Trim();
        ClosingVerse =
            closingVerse.Trim();
        ImageUrl =
            NormalizeOptionalText(
                imageUrl);
        IsActive = isActive;

        MarkAsUpdated();
    }

    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        MarkAsUpdated();
    }

    private static void ValidateYear(
        int year)
    {
        if (year < 2000 ||
            year > 2100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(year),
                "Year must be a realistic calendar year.");
        }
    }

    private static string? NormalizeOptionalText(
        string? value)
    {
        return string.IsNullOrWhiteSpace(
            value)
            ? null
            : value.Trim();
    }
}