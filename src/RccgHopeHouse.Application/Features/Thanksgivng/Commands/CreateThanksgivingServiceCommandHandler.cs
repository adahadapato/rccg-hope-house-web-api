using MediatR;
using RccgHopeHouse.Application.Features.Thanksgiving.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Thanksgiving.Commands;

/// <summary>
/// Handler for creating a thanksgiving service manually.
/// Uses domain factory method for entity creation.
/// </summary>
public class CreateThanksgivingServiceCommandHandler : IRequestHandler<CreateThanksgivingServiceCommand, ThanksgivingServiceDto>
{
    private readonly IThanksgivingRepository _repository;

    public CreateThanksgivingServiceCommandHandler(IThanksgivingRepository repository) => _repository = repository;

    public async Task<ThanksgivingServiceDto> Handle(
        CreateThanksgivingServiceCommand request,
        CancellationToken cancellationToken)
    {
        // ✅ Use domain factory method, not constructor
        var service = ThanksgivingService.Create(
            title: request.Title,
            videoUrl: request.VideoUrl,
            publishedAt: request.ServiceMonth, // Using ServiceMonth as publishedAt for manual entries
            thumbnailUrl: request.ThumbnailUrl,
            description: request.Description,
            isAutoSynced: false);

        await _repository.AddAsync(service, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new ThanksgivingServiceDto(
            service.Id, service.Title, service.VideoUrl, service.ThumbnailUrl, service.ServiceMonth, service.IsAutoSynced);
    }
}