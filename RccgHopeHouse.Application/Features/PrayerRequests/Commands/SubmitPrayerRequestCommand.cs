using MediatR;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Commands;

/// <summary>
/// Represents a request to submit a new prayer request.
/// </summary>
/// <param name="Content">
/// The prayer request message submitted by the requester.
/// </param>
/// <param name="IsAnonymous">
/// Indicates whether the requester wishes to remain anonymous.
/// </param>
/// <param name="RequesterName">
/// The requester's name when the submission is not anonymous.
/// </param>
/// <param name="PhoneNumber">
/// The requester's optional phone number.
/// </param>
/// <param name="RequesterEmail">
/// The requester's optional email address.
/// </param>
/// <remarks>
/// Anonymous submissions do not retain requester contact information.
/// Validation and creation rules are enforced by the
/// <c>PrayerRequest</c> domain entity.
/// </remarks>
public record SubmitPrayerRequestCommand(
    string Content,
    bool IsAnonymous,
    string? RequesterName,
    string? PhoneNumber,
    string? RequesterEmail) : IRequest<Unit>;