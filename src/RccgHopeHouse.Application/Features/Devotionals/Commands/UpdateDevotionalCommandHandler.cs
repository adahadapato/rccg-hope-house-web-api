using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Devotionals.Commands;

public class UpdateDevotionalCommandHandler
    : IRequestHandler<UpdateDevotionalCommand, DevotionalDto>
{
    private readonly IDevotionalRepository _repository;

    public UpdateDevotionalCommandHandler(IDevotionalRepository repository)
    {
        _repository = repository;
    }

    public async Task<DevotionalDto> Handle(
        UpdateDevotionalCommand request,
        CancellationToken cancellationToken)
    {
        var devotional = await _repository.GetByIdForAdminAsync(
            request.Id,
            cancellationToken)
            ?? throw new NotFoundException(
                nameof(Devotional),
                request.Id);

        devotional.Update(
            devotionalDate: request.DevotionalDate,
            theme: request.Theme,
            scriptureReference: request.ScriptureReference,
            passageId: request.PassageId,
            thought: request.Thought,
            commentaryPoints: request.CommentaryPoints,
            prayerPoints: request.PrayerPoints,
            declaration: request.Declaration);

        await _repository.UpdateAsync(devotional, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return DevotionalDto.FromEntity(devotional);
    }
}