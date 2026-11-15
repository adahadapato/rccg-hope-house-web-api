using MediatR;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

/// <summary>
/// Handles changes to the public publication state of an
/// individual service broadcast.
/// </summary>
public sealed class SetServiceBroadcastPublishedCommandHandler
    : IRequestHandler<
        SetServiceBroadcastPublishedCommand,
        ServiceBroadcastDto>
{
    private readonly IServiceBroadcastRepository _repository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="SetServiceBroadcastPublishedCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve and persist service broadcasts.
    /// </param>
    public SetServiceBroadcastPublishedCommandHandler(
        IServiceBroadcastRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Publishes or unpublishes an individual service broadcast.
    /// </summary>
    /// <param name="request">
    /// Command containing the broadcast identifier and required
    /// publication state.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// The updated service broadcast.
    /// </returns>
    public async Task<ServiceBroadcastDto> Handle(
        SetServiceBroadcastPublishedCommand request,
        CancellationToken ct)
    {
        var broadcast =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(ServiceBroadcast),
                request.Id);

        broadcast.SetPublished(
            request.IsPublished);

        await _repository.SaveChangesAsync(
            ct);

        return ServiceBroadcastDto.FromEntity(
            broadcast);
    }
}