using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Devotionals.Queries;

public class GetLatestDevotionalQueryHandler
    : IRequestHandler<GetLatestDevotionalQuery, DevotionalDto>
{
    private readonly IDevotionalRepository _repository;

    public GetLatestDevotionalQueryHandler(IDevotionalRepository repository)
    {
        _repository = repository;
    }

    public async Task<DevotionalDto> Handle(
        GetLatestDevotionalQuery request,
        CancellationToken cancellationToken)
    {
        var devotional = await _repository.GetLatestPublishedAsync(
            request.Today,
            cancellationToken)
            ?? throw new NotFoundException(
                nameof(Devotional),
                request.Today);

        return DevotionalDto.FromEntity(devotional);
    }
}