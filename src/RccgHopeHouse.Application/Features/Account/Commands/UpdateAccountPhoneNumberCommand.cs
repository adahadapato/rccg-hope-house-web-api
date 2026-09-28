using MediatR;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Account.Commands;

/// <summary>
/// Updates the phone number of the
/// currently authenticated user.
/// </summary>
public sealed record UpdateAccountPhoneNumberCommand(
    string UserId,
    string? PhoneNumber)
    : IRequest<AccountProfileResult>;