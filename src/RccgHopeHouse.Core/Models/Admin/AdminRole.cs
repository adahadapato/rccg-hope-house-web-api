namespace RccgHopeHouse.Core.Models.Admin;

/// <summary>
/// Represents an application role available for assignment
/// to users through the administration area.
/// </summary>
public sealed class AdminRole
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }
}