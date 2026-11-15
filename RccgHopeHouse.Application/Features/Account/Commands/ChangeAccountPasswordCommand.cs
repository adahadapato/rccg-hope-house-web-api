using MediatR;

namespace RccgHopeHouse.Application.Features.Account.Commands;

/// <summary>
/// Changes the password of the currently authenticated user
/// after verifying their existing password.
/// </summary>
public sealed record ChangeAccountPasswordCommand(
    string UserId,
    string CurrentPassword,
    string NewPassword)
    : IRequest;