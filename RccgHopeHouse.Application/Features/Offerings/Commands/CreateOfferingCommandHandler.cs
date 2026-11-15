using MediatR;
using RccgHopeHouse.Application.Features.Offerings.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Offerings.Commands;

/// <summary>
/// Handles creation of a new offering.
/// </summary>
public class CreateOfferingCommandHandler
    : IRequestHandler<CreateOfferingCommand, OfferingDto>
{
    private readonly IOfferingRepository _offeringRepository;
    private readonly IGivingTypeRepository _givingTypeRepository;

    public CreateOfferingCommandHandler(
        IOfferingRepository offeringRepository,
        IGivingTypeRepository givingTypeRepository)
    {
        _offeringRepository = offeringRepository;
        _givingTypeRepository = givingTypeRepository;
    }

    public async Task<OfferingDto> Handle(
    CreateOfferingCommand request,
    CancellationToken ct)
    {
        var givingType = await _givingTypeRepository
            .GetByIdAsync(request.GivingTypeId, ct)
            ?? throw new NotFoundException(
                nameof(GivingType),
                request.GivingTypeId);

        if (!givingType.IsActive)
            throw new InvalidOperationException(
                "The selected giving type is not currently available.");

        var offering = Offering.Create(
            givingTypeId: request.GivingTypeId,
            amount: request.Amount,
            isAnonymous: request.IsAnonymous,
            fullName: request.FullName,
            email: request.Email,
            messageReference: request.MessageReference);

        await _offeringRepository.AddAsync(offering, ct);
        await _offeringRepository.SaveChangesAsync(ct);

        return new OfferingDto(
            Id: offering.Id,
            GivingTypeId: offering.GivingTypeId,
            GivingTypeName: givingType.Name,
            Amount: offering.Amount,
            PaymentStatus: offering.PaymentStatus,
            PaymentReference: offering.PaymentReference,
            FullName: offering.FullName,
            Email: offering.Email?.Value,
            IsAnonymous: offering.IsAnonymous,
            MessageReference: offering.MessageReference,
            CreatedAt: offering.CreatedAt);
    }
}