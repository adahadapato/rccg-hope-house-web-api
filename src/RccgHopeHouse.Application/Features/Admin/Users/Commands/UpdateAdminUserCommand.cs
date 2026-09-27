using MediatR;

namespace RccgHopeHouse.Application.Features.Admin.Users.Commands;

/// <summary>
/// Updates an existing application user's profile information.
/// </summary>
public sealed record UpdateAdminUserCommand(
    string UserId,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber)
    : IRequest;
