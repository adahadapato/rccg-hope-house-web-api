namespace RccgHopeHouse.Application.Features.Celebrations.Dtos;

/// <summary>
/// Contains the limited celebration details displayed
/// to a member before submitting a publication decision.
/// </summary>
/// <param name="Occasion">
/// The celebration occasion.
/// </param>
/// <param name="CelebrationDate">
/// The scheduled date of the celebration.
/// </param>
/// <param name="ExpiresAt">
/// The UTC date and time at which the invitation expires.
/// </param>
public sealed record CelebrationInvitationDetailsDto(
    string Occasion,
    DateOnly CelebrationDate,
    DateTime ExpiresAt);
