using Microsoft.AspNetCore.Http;
using RccgHopeHouse.Core.Interfaces;
using System.Security.Claims;

namespace RccgHopeHouse.Infrastructure.Services;

/// <summary>
/// Implements <see cref="ICurrentUserService"/> to expose authenticated user context to Application handlers.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes with HTTP context accessor for request-scoped user data.
    /// </summary>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor) =>
        _httpContextAccessor = httpContextAccessor;

    /// <inheritdoc />
    public Guid? UserId
    {
        get
        {
            var id = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(id, out var parsed) ? parsed : null;
        }
    }

    /// <inheritdoc />
    public string? UserName => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

    /// <inheritdoc />
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc />
    public bool IsInRole(string role) => _httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;
}