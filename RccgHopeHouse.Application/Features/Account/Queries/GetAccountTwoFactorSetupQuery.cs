using MediatR;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Account.Queries;

public sealed record GetAccountTwoFactorSetupQuery(
    string UserId)
    : IRequest<TwoFactorSetupResult>;