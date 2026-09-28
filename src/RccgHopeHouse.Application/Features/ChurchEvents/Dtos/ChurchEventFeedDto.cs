using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchEvents.Dtos;

/// <summary>
/// Lightweight DTO used by the public
/// Upcoming Events section.
/// </summary>
public record ChurchEventFeedDto(
    Guid Id,
    string Title,
    ServiceCategory Category,
    DateTime StartDateTime,
    DateTime? EndDateTime,
    string? Description,
    string? Location,
    string? Icon,
    string? Color,
    string? RegistrationUrl,
    string RegistrationButtonText,
    string? ImageUrl,
    int DisplayOrder);