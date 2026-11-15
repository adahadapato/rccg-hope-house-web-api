using MediatR;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Account.Commands;

public sealed record RemoveAccountProfileImageCommand(
    string UserId)
    : IRequest<AccountProfileResult>;