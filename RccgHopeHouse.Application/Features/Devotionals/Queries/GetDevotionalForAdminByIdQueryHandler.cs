using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Devotionals.Queries;

public class GetDevotionalForAdminByIdQueryHandler
    : IRequestHandler<GetDevotionalForAdminByIdQuery, DevotionalDto>
{
    private readonly IDevotionalRepository _repository;

    public GetDevotionalForAdminByIdQueryHandler(IDevotionalRepository repository)
    {
        _repository = repository;
    }

    public async Task<DevotionalDto> Handle(
        GetDevotionalForAdminByIdQuery request,
        CancellationToken cancellationToken)
    {
        var devotional = await _repository.GetByIdForAdminAsync(
            request.Id,
            cancellationToken)
            ?? throw new NotFoundException(
                nameof(Devotional),
                request.Id);

        return DevotionalDto.FromEntity(devotional);
    }
}