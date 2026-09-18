using MediatR;

namespace RccgHopeHouse.Application.Features.Members.Commands;

public record DeactivateMemberCommand(Guid Id) : IRequest<Unit>;