using MediatR;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Admin.Users.Commands;

/// <summary>
/// Generates an email confirmation token and sends
/// a verification email to an application user.
/// </summary>
public sealed record SendAdminUserVerificationEmailCommand(
    string UserId,
    string VerificationBaseUrl)
    : IRequest<EmailResult>;