using MediatR;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Members.Commands;

public class DeactivateMemberCommandHandler : IRequestHandler<DeactivateMemberCommand, Unit>
{
    private readonly IMemberRepository _repository;

    public DeactivateMemberCommandHandler(IMemberRepository repository) => _repository = repository;

    public async Task<Unit> Handle(DeactivateMemberCommand request, CancellationToken ct)
    {
        var member = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Member), request.Id);

        member.Deactivate();

        await _repository.UpdateAsync(member, ct);
        await _repository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}