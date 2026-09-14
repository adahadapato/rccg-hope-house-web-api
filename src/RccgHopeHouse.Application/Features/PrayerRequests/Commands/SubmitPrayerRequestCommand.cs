using MediatR;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Commands;

/// <summary>
/// Command to submit a new prayer request from the public contact form.
/// Creates the request in "Pending" status for pastoral review.
/// </summary>
public record SubmitPrayerRequestCommand(
    string RequesterName,
    string Content,
    string? RequesterEmail,
    string PhoneNumber) : IRequest<Unit>;