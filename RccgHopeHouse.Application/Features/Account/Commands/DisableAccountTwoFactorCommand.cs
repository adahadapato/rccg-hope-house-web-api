using MediatR;

namespace RccgHopeHouse.Application.Features.Account.Commands;

public sealed record DisableAccountTwoFactorCommand(
    string UserId)
    : IRequest;