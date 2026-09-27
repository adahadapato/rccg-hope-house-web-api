using MediatR;

namespace RccgHopeHouse.Application.Features.Admin.Users.Commands;

public sealed record DeleteAdminUserCommand(
 string UserId,
 string CurrentUserId) : IRequest;
