using MediatR;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

public class CreateServiceBroadcastCommandHandler : IRequestHandler<CreateServiceBroadcastCommand, ServiceBroadcastDto>
{
    private readonly IServiceBroadcastRepository _repository;

    public CreateServiceBroadcastCommandHandler(IServiceBroadcastRepository repository) => _repository = repository;

    public async Task<ServiceBroadcastDto> Handle(CreateServiceBroadcastCommand request, CancellationToken ct)
    {
        var broadcast = ServiceBroadcast.Create(
            request.Category,
            request.Title,
            request.YoutubeUrl,
            request.ServiceMonth,
            request.Description,
            request.Theme,
            request.IsLive);

        await _repository.AddAsync(broadcast, ct);
        await _repository.SaveChangesAsync(ct);

        return ServiceBroadcastDto.FromEntity(broadcast);
    }
}