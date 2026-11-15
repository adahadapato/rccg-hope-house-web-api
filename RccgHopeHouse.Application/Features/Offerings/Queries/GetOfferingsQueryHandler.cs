using MediatR;
using RccgHopeHouse.Application.Features.Offerings.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Offerings.Queries;

public class GetOfferingsQueryHandler
    : IRequestHandler<GetOfferingsQuery, IReadOnlyList<OfferingDto>>
{
    private readonly IOfferingRepository _repository;

    public GetOfferingsQueryHandler(IOfferingRepository repository) =>
        _repository = repository;

    public async Task<IReadOnlyList<OfferingDto>> Handle(
        GetOfferingsQuery request,
        CancellationToken ct)
    {
        var offerings = await _repository.GetAllAsync(
            request.Skip,
            request.Take,
            ct);

        return offerings
            .Select(OfferingDto.FromEntity)
            .ToList();
    }
}