using MediatR;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Account.Queries;

public sealed record GetAccountTwoFactorStatusQuery(
    string UserId)
    : IRequest<TwoFactorStatusResult>;