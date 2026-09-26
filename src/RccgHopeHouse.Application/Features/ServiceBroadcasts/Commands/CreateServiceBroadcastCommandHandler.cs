using MediatR;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

public class CreateServiceBroadcastCommandHandler
    : IRequestHandler<CreateServiceBroadcastCommand, ServiceBroadcastDto>
{
    private readonly IServiceBroadcastRepository _repository;
    private readonly IChurchServiceRepository _churchServiceRepository;

    public CreateServiceBroadcastCommandHandler(
        IServiceBroadcastRepository repository,
        IChurchServiceRepository churchServiceRepository)
    {
        _repository = repository;
        _churchServiceRepository = churchServiceRepository;
    }

    public async Task<ServiceBroadcastDto> Handle(
        CreateServiceBroadcastCommand request,
        CancellationToken ct)
    {
        if (request.ChurchServiceId == Guid.Empty)
        {
            throw new ArgumentException(
                "A church service is required.",
                nameof(request.ChurchServiceId));
        }

        var churchService =
            await _churchServiceRepository.GetByIdAsync(
                request.ChurchServiceId,
                ct);

        if (churchService is null)
        {
            throw new KeyNotFoundException(
                $"Church service '{request.ChurchServiceId}' was not found.");
        }

        var broadcast = ServiceBroadcast.Create(
            request.ChurchServiceId,
            churchService.Category,
            request.Title,
            request.YoutubeUrl,
            request.ServiceMonth,
            request.Description,
            request.Theme,
            request.IsLive);

        await _repository.AddAsync(
            broadcast,
            ct);

        await _repository.SaveChangesAsync(
            ct);

        return ServiceBroadcastDto.FromEntity(
            broadcast);
    }
}