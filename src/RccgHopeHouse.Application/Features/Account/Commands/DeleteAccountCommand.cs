using MediatR;

namespace RccgHopeHouse.Application.Features.Account.Commands;

public sealed record DeleteAccountCommand(
    string UserId,
    string CurrentPassword)
    : IRequest;