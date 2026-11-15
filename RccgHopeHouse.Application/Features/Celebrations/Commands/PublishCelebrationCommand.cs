
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Represents a request to publish an administrator-approved celebration.
/// </summary>
/// <param name="CelebrationId">The unique identifier of the celebration to publish.</param>
/// <remarks>
/// Only celebrations with ReadyToPublish status can be published.
/// This command is intended for an authorised administrator or trusted publication workflow.
/// </remarks>
public sealed record PublishCelebrationCommand(Guid CelebrationId) : IRequest<CelebrationDto>;
