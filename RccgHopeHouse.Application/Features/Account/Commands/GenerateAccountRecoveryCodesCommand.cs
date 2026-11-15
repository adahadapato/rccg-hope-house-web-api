using MediatR;

namespace RccgHopeHouse.Application.Features.Account.Commands;

public sealed record GenerateAccountRecoveryCodesCommand(
    string UserId)
    : IRequest<IReadOnlyCollection<string>>;