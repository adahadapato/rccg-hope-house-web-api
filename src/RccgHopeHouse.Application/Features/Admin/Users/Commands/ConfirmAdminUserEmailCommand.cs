using MediatR;

namespace RccgHopeHouse.Application.Features.Admin.Users.Commands;

/// <summary>
/// Confirms an administrative user's email address
/// using an email confirmation token.
/// </summary>
public sealed record ConfirmAdminUserEmailCommand(
    string UserId,
    string Token)
    : IRequest;
