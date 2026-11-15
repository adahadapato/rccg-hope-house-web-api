using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Devotionals.Queries;

public class GetDevotionalsForAdminQueryHandler
    : IRequestHandler<GetDevotionalsForAdminQuery, IReadOnlyList<DevotionalDto>>
{
    private readonly IDevotionalRepository _repository;

    public GetDevotionalsForAdminQueryHandler(IDevotionalRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<DevotionalDto>> Handle(
        GetDevotionalsForAdminQuery request,
        CancellationToken cancellationToken)
    {
        var devotionals = await _repository.GetAllForAdminAsync(
            cancellationToken);

        return devotionals
            .Select(DevotionalDto.FromEntity)
            .ToList();
    }
}