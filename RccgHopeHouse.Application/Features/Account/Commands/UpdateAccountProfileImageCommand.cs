using MediatR;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Account.Commands;

public sealed record UpdateAccountProfileImageCommand(
    string UserId,
    byte[] ImageData,
    string ContentType)
    : IRequest<AccountProfileResult>;