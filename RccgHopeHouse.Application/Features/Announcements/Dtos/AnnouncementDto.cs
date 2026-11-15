using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.Announcements.Dtos;

/// <summary>
/// Full announcement used by the admin News &amp; Updates page.
/// </summary>
public record AnnouncementDto(
    Guid Id,
    string Title,
    string? Summary,
    string? Body,
    AnnouncementType Type,
    string? ImagePath,
    string? ThumbnailPath,
    string? LinkUrl,
    string? LinkText,
    DateTime? EventStartsAt,
    DateTime? EventEndsAt,
    string? VenueName,
    string? VenueAddress,
    DateTime StartsAt,
    DateTime? EndsAt,
    bool IsPublished,
    bool ShowInBanner,
    int DisplayOrder,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

/// <summary>
/// Public announcement shown in the floating banner.
/// <see cref="Version"/> changes whenever the announcement is edited,
/// so visitors who dismissed the old version see the new one.
/// </summary>
public record AnnouncementFeedDto(
    Guid Id,
    string Title,
    string? Summary,
    string? Body,
    AnnouncementType Type,
    string? ImagePath,
    string? ThumbnailPath,
    string? LinkUrl,
    string? LinkText,
    DateTime? EventStartsAt,
    DateTime? EventEndsAt,
    string? VenueName,
    string? VenueAddress,
    DateTime StartsAt,
    DateTime? EndsAt,
    bool ShowInBanner,
    string Version);

public static class AnnouncementMappings
{
    public static AnnouncementDto ToDto(this Announcement a) => new(
        a.Id,
        a.Title,
        a.Summary,
        a.Body,
        a.Type,
        a.ImagePath,
        a.ThumbnailPath,
        a.LinkUrl,
        a.LinkText,
        a.EventStartsAt,
        a.EventEndsAt,
        a.VenueName,
        a.VenueAddress,
        a.StartsAt,
        a.EndsAt,
        a.IsPublished,
        a.ShowInBanner,
        a.DisplayOrder,
        a.CreatedAt,
        a.UpdatedAt);

    public static AnnouncementFeedDto ToFeedDto(this Announcement a) => new(
        a.Id,
        a.Title,
        a.Summary,
        a.Body,
        a.Type,
        a.ImagePath,
        a.ThumbnailPath,
        a.LinkUrl,
        a.LinkText,
        a.EventStartsAt,
        a.EventEndsAt,
        a.VenueName,
        a.VenueAddress,
        a.StartsAt,
        a.EndsAt,
        a.ShowInBanner,
        (a.UpdatedAt ?? a.CreatedAt).Ticks.ToString());
}
