namespace RccgHopeHouse.Application.Features.Admin.Roles.Dtos;

/// <summary>
/// Represents an application role returned to the
/// administration interface.
/// </summary>
public sealed record AdminRoleDto(
    string Id,
    string Name,
    string? Description);