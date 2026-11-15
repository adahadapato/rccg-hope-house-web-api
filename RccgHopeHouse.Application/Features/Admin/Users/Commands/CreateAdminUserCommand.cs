using MediatR;

namespace RccgHopeHouse.Application.Features.Admin.Users.Commands;

/// <summary>
/// Creates a new application user from the administration area.
/// </summary>
public sealed record CreateAdminUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Password,
    IReadOnlyCollection<string> Roles)
    : IRequest<string>;
