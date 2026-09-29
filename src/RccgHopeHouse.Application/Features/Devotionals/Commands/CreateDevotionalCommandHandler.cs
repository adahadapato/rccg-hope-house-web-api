using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Devotionals.Commands;

public class CreateDevotionalCommandHandler
    : IRequestHandler<CreateDevotionalCommand, DevotionalDto>
{
    private readonly IDevotionalRepository _repository;

    public CreateDevotionalCommandHandler(IDevotionalRepository repository)
    {
        _repository = repository;
    }

    public async Task<DevotionalDto> Handle(
        CreateDevotionalCommand request,
        CancellationToken cancellationToken)
    {
        var devotional = Devotional.Create(
            devotionalDate: request.DevotionalDate,
            theme: request.Theme,
            scriptureReference: request.ScriptureReference,
            passageId: request.PassageId,
            thought: request.Thought,
            commentaryPoints: request.CommentaryPoints,
            prayerPoints: request.PrayerPoints,
            declaration: request.Declaration);

        await _repository.AddAsync(devotional, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return DevotionalDto.FromEntity(devotional);
    }
}