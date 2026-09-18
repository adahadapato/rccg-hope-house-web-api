using MediatR;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Commands;

public record SubmitPrayerRequestCommand(
    string Content,
    bool IsAnonymous,
    string? RequesterName,
    string? PhoneNumber,
    string? RequesterEmail) : IRequest<Unit>;