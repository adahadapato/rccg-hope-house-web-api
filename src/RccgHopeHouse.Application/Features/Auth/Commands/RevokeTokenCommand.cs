using MediatR;

namespace RccgHopeHouse.Application.Features.Auth.Commands;

/// <summary>
/// Command to revoke a refresh token, effectively logging out the user.
/// In production, this would mark the token as revoked in a distributed cache or database.
/// </summary>
public record RevokeTokenCommand(string RefreshToken) : IRequest<Unit>;