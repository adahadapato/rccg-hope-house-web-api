using MediatR;
using RccgHopeHouse.Core.Results.AI;

namespace RccgHopeHouse.Application.Features.Devotionals.Commands;

/// <summary>
/// Requests automatic synchronisation of an Open Heavens devotional
/// for the specified UK calendar date.
/// </summary>
/// <param name="DevotionalDate">
/// The date of the devotional to synchronise.
/// </param>
public sealed record SynchronizeOpenHeavensDevotionalsCommand(
    DateOnly DevotionalDate)
    : IRequest<OpenHeavensDevotionalSynchronizationResult>;