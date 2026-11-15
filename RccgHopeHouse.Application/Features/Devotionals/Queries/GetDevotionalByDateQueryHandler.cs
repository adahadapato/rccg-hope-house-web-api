using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Devotionals.Queries;

public class GetDevotionalByDateQueryHandler
    : IRequestHandler<GetDevotionalByDateQuery, DevotionalDto>
{
    private readonly IDevotionalRepository _repository;

    public GetDevotionalByDateQueryHandler(IDevotionalRepository repository)
    {
        _repository = repository;
    }

    public async Task<DevotionalDto> Handle(
        GetDevotionalByDateQuery request,
        CancellationToken cancellationToken)
    {
        var devotional = await _repository.GetPublishedByDateAsync(
            request.DevotionalDate,
            request.Today,
            cancellationToken)
            ?? throw new NotFoundException(
                nameof(Devotional),
                request.DevotionalDate);

        return DevotionalDto.FromEntity(devotional);
    }
}