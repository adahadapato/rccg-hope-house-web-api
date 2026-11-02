using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Exceptions;

namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// A birthday wish left by a visitor on a
/// "Celebrants of the Month" gallery photo.
///
/// Every wish is checked before it appears. Only approved
/// wishes are shown on the website, and wishes are never
/// sent to members by SMS or email.
/// </summary>
public class GalleryImageComment : BaseEntity
{
    public const int AuthorNameMaxLength = 60;
    public const int MessageMaxLength = 500;
    public const int DecisionReasonMaxLength = 300;
    public const int ModeratedByMaxLength = 150;

    /// <summary>The celebrant photo this wish belongs to.</summary>
    public Guid GalleryImageId { get; private set; }

    public GalleryImage GalleryImage { get; private set; } = null!;

    /// <summary>Name the visitor typed, e.g. "Sister Grace".</summary>
    public string AuthorName { get; private set; } = string.Empty;

    /// <summary>The wish itself (plain text).</summary>
    public string Message { get; private set; } = string.Empty;

    public BirthdayWishStatus Status { get; private set; } = BirthdayWishStatus.Pending;

    /// <summary>Who made the latest decision.</summary>
    public BirthdayWishDecisionSource DecisionSource { get; private set; } = BirthdayWishDecisionSource.System;

    /// <summary>Short explanation of the latest decision.</summary>
    public string? DecisionReason { get; private set; }

    /// <summary>Admin who made the latest decision, when an admin did.</summary>
    public string? ModeratedBy { get; private set; }

    /// <summary>When the latest decision was made (UTC).</summary>
    public DateTime? ModeratedAt { get; private set; }

    private GalleryImageComment()
    {
    }

    /// <summary>
    /// Creates a new wish. It starts as Pending (hidden)
    /// until a decision is applied.
    /// </summary>
    public static GalleryImageComment Create(
        Guid galleryImageId,
        string authorName,
        string message)
    {
        if (galleryImageId == Guid.Empty)
        {
            throw new DomainException("A celebrant photo is required.");
        }

        var name = NormalizeText(authorName);
        var text = NormalizeText(message);

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Your name is required.");
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new DomainException("Please write a birthday wish.");
        }

        if (name.Length > AuthorNameMaxLength)
        {
            throw new DomainException(
                $"Name cannot be longer than {AuthorNameMaxLength} characters.");
        }

        if (text.Length > MessageMaxLength)
        {
            throw new DomainException(
                $"Wish cannot be longer than {MessageMaxLength} characters.");
        }

        return new GalleryImageComment
        {
            GalleryImageId = galleryImageId,
            AuthorName = name,
            Message = text,
            Status = BirthdayWishStatus.Pending,
            DecisionSource = BirthdayWishDecisionSource.System
        };
    }

    /// <summary>
    /// Records a moderation decision (from the AI, an admin
    /// or a built-in rule).
    /// </summary>
    public void ApplyDecision(
        BirthdayWishStatus status,
        BirthdayWishDecisionSource source,
        string? reason,
        string? moderatedBy = null)
    {
        if (!Enum.IsDefined(status))
        {
            throw new DomainException("Invalid birthday wish status.");
        }

        Status = status;
        DecisionSource = source;
        DecisionReason = Truncate(NormalizeOptional(reason), DecisionReasonMaxLength);
        ModeratedBy = Truncate(NormalizeOptional(moderatedBy), ModeratedByMaxLength);
        ModeratedAt = DateTime.UtcNow;

        MarkAsUpdated();
    }

    /// <summary>
    /// Trims the text and collapses runs of blank lines and
    /// spaces so a wish cannot be padded out to fill the page.
    /// </summary>
    private static string NormalizeText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var lines = value
            .Replace("\r\n", "\n")
            .Replace('\r', '\n')
            .Split('\n')
            .Select(line => string.Join(
                ' ',
                line.Split(' ', StringSplitOptions.RemoveEmptyEntries)));

        var result = string.Join('\n', lines).Trim();

        while (result.Contains("\n\n\n", StringComparison.Ordinal))
        {
            result = result.Replace("\n\n\n", "\n\n", StringComparison.Ordinal);
        }

        return result;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? Truncate(string? value, int maxLength) =>
        value is null || value.Length <= maxLength
            ? value
            : value[..maxLength];
}
