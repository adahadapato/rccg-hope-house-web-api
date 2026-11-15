using MediatR;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Account.Queries;

/// <summary>
/// Gets profile and account information for the
/// currently authenticated user.
/// </summary>
public sealed record GetAccountProfileQuery(
    string UserId)
    : IRequest<AccountProfileResult>;