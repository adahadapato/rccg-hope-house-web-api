using MediatR;
using RccgHopeHouse.Application.Features.Offerings.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Offerings.Queries;

public class GetOfferingByIdQueryHandler
    : IRequestHandler<GetOfferingByIdQuery, OfferingDto>
{
    private readonly IOfferingRepository _repository;

    public GetOfferingByIdQueryHandler(IOfferingRepository repository) =>
        _repository = repository;

    public async Task<OfferingDto> Handle(
        GetOfferingByIdQuery request,
        CancellationToken ct)
    {
        var offering = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Offering), request.Id);

        return OfferingDto.FromEntity(offering);
    }
}