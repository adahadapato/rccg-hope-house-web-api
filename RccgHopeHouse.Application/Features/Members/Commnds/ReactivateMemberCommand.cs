using MediatR;

namespace RccgHopeHouse.Application.Features.Members.Commands;

public record ReactivateMemberCommand(Guid Id) : IRequest<Unit>;