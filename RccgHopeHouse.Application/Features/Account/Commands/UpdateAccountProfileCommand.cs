using MediatR;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Account.Commands;

/// <summary>
/// Updates the first and last name of the
/// currently authenticated user.
/// </summary>
public sealed record UpdateAccountProfileCommand(
    string UserId,
    string FirstName,
    string LastName)
    : IRequest<AccountProfileResult>;