using MediatR;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

public class UpdateServiceBroadcastCommandHandler : IRequestHandler<UpdateServiceBroadcastCommand, ServiceBroadcastDto>
{
    private readonly IServiceBroadcastRepository _repository;

    public UpdateServiceBroadcastCommandHandler(IServiceBroadcastRepository repository) => _repository = repository;

    public async Task<ServiceBroadcastDto> Handle(UpdateServiceBroadcastCommand request, CancellationToken ct)
    {
        var broadcast = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(ServiceBroadcast), request.Id);

        broadcast.Update(request.Title, request.YoutubeUrl, request.Description, request.Theme, request.IsLive);

        await _repository.UpdateAsync(broadcast, ct);
        await _repository.SaveChangesAsync(ct);

        return ServiceBroadcastDto.FromEntity(broadcast);
    }
}