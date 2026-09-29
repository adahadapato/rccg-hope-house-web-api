using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Devotionals.Commands;

public class PublishDevotionalCommandHandler
    : IRequestHandler<PublishDevotionalCommand, DevotionalDto>
{
    private readonly IDevotionalRepository _repository;

    public PublishDevotionalCommandHandler(IDevotionalRepository repository)
    {
        _repository = repository;
    }

    public async Task<DevotionalDto> Handle(
        PublishDevotionalCommand request,
        CancellationToken cancellationToken)
    {
        var devotional = await _repository.GetByIdForAdminAsync(
            request.Id,
            cancellationToken)
            ?? throw new NotFoundException(
                nameof(Devotional),
                request.Id);

        devotional.Publish();

        await _repository.UpdateAsync(devotional, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return DevotionalDto.FromEntity(devotional);
    }
}