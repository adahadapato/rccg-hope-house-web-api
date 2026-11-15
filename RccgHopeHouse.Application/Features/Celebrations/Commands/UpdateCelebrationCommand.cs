
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Represents a request to update an existing pending
/// birthday or wedding anniversary celebration.
/// </summary>
/// <param name="Id">
/// The unique identifier of the celebration to update.
/// </param>
/// <param name="GalleryCategoryId">
/// The identifier of the existing gallery category.
/// </param>
/// <param name="Occasion">
/// The celebration occasion: Birthday or WeddingAnniversary.
/// </param>
/// <param name="CelebrationDate">
/// The updated date of the celebration.
/// </param>
/// <remarks>
/// Updates are permitted only while the celebration
/// has Pending status.
///
/// Member identity, consent, approval and publication
/// information cannot be modified through this command.
///
/// Changing the celebration details invalidates any
/// previously sent invitation, requiring a new invitation
/// before member consent can be collected.
/// </remarks>
public sealed record UpdateCelebrationCommand(
    Guid Id,
    Guid GalleryCategoryId,
    string Occasion,
    DateOnly CelebrationDate
) : IRequest<CelebrationDto>;
