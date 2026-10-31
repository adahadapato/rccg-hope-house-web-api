
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Represents a request to create a new birthday or
/// wedding anniversary celebration for a church member.
/// </summary>
/// <param name="MemberId">
/// The unique identifier of the church member.
/// </param>
/// <param name="GalleryCategoryId">
/// The identifier of the existing gallery category
/// associated with the celebration.
/// </param>
/// <param name="Occasion">
/// The type of celebration.
/// Supported values are Birthday and WeddingAnniversary.
/// </param>
/// <param name="CelebrationDate">
/// The date on which the celebration will take place.
/// </param>
/// <remarks>
/// Creating a celebration does not automatically authorise
/// publication of member details or photographs.
///
/// New celebrations begin with Pending status and must
/// complete the member consent and administrator approval
/// workflow before publication.
/// </remarks>
public sealed record CreateCelebrationCommand(
    Guid MemberId,
    Guid GalleryCategoryId,
    string Occasion,
    DateOnly CelebrationDate
) : IRequest<CelebrationDto>;
