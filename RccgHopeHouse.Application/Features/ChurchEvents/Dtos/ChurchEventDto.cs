using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchEvents.Dtos;

/// <summary>
/// Detailed DTO for church event admin views
/// and public Upcoming Events responses.
/// </summary>
public record ChurchEventDto(
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
    bool IsActive,
    int DisplayOrder);