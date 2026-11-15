using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.BirthdayWishes.Dtos;

/// <summary>An approved wish shown under a celebrant photo.</summary>
public record BirthdayWishDto(
    Guid Id,
    string AuthorName,
    string Message,
    DateTime CreatedAt);

/// <summary>Number of approved wishes on one photo.</summary>
public record BirthdayWishCountDto(
    Guid GalleryImageId,
    int Count);

/// <summary>
/// What the visitor is told after sending a wish.
/// <see cref="Wish"/> is only set when it was published straight away.
/// </summary>
public record SubmitBirthdayWishResultDto(
    BirthdayWishStatus Status,
    string Message,
    BirthdayWishDto? Wish);

/// <summary>A wish as shown on the admin Birthday Wishes page.</summary>
public record AdminBirthdayWishDto(
    Guid Id,
    Guid GalleryImageId,
    string? ImageTitle,
    string? ImageThumbnailPath,
    string AuthorName,
    string Message,
    BirthdayWishStatus Status,
    BirthdayWishDecisionSource DecisionSource,
    string? DecisionReason,
    string? ModeratedBy,
    DateTime? ModeratedAt,
    DateTime CreatedAt);

/// <summary>Counts for the admin page tabs and the bell icon.</summary>
public record BirthdayWishStatsDto(
    int PendingCount,
    int ApprovedCount,
    int RejectedCount);

public static class BirthdayWishMappings
{
    public static BirthdayWishDto ToDto(this GalleryImageComment c) => new(
        c.Id,
        c.AuthorName,
        c.Message,
        c.CreatedAt);

    public static AdminBirthdayWishDto ToAdminDto(this GalleryImageComment c) => new(
        c.Id,
        c.GalleryImageId,
        c.GalleryImage?.Title,
        c.GalleryImage?.ThumbnailPath,
        c.AuthorName,
        c.Message,
        c.Status,
        c.DecisionSource,
        c.DecisionReason,
        c.ModeratedBy,
        c.ModeratedAt,
        c.CreatedAt);
}
