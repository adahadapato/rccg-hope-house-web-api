using MediatR;
using RccgHopeHouse.Application.Features.Auth.Dtos;

namespace RccgHopeHouse.Application.Features.Auth.Commands;

/// <summary>
/// Command to authenticate a user and return JWT tokens.
/// </summary>
public record LoginCommand(string Email, string Password) : IRequest<AuthTokensDto>;