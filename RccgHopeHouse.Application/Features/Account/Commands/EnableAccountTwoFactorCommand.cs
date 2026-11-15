using MediatR;

namespace RccgHopeHouse.Application.Features.Account.Commands;

public sealed record EnableAccountTwoFactorCommand(
    string UserId,
    string VerificationCode)
    : IRequest<IReadOnlyCollection<string>>;