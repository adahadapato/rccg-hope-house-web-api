
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Application.Features.Celebrations.Dtos;

/// <summary>
/// Represents a church member's birthday or wedding
/// anniversary celebration returned by the application layer.
/// </summary>
/// <param name="Id">
/// The unique identifier of the celebration.
/// </param>
/// <param name="MemberId">
/// The unique identifier of the associated church member.
/// </param>
/// <param name="MemberName">
/// The full name of the church member.
/// </param>
/// <param name="GalleryCategoryId">
/// The identifier of the gallery category associated
/// with the celebration.
/// </param>
/// <param name="GalleryCategoryName">
/// The name of the associated gallery category.
/// </param>
/// <param name="Occasion">
/// The celebration occasion: Birthday or WeddingAnniversary.
/// </param>
/// <param name="CelebrationDate">
/// The date on which the celebration takes place.
/// </param>
/// <param name="GalleryImageId">
/// The identifier of the existing gallery image selected
/// for this celebration, if a photograph has been selected.
/// </param>
/// <param name="PhotoUrl">
/// The legacy photograph URL, retained for compatibility
/// with existing celebration records.
/// </param>
/// <param name="PhotoPublicationConsent">
/// Indicates whether the member has explicitly consented
/// to publication of the selected photograph.
/// </param>
/// <param name="Status">
/// The current celebration workflow status.
/// </param>
/// <param name="InvitationSentAt">
/// The UTC date and time the invitation was sent.
/// </param>
/// <param name="RespondedAt">
/// The UTC date and time the member responded.
/// </param>
/// <param name="ApprovedByAdminAt">
/// The UTC date and time administrator approval was granted.
/// </param>
/// <param name="PublishedAt">
/// The UTC date and time the celebration was published.
/// </param>
/// <param name="WithdrawnAt">
/// The UTC date and time publication permission was withdrawn.
/// </param>
public record CelebrationDto(
    Guid Id,
    Guid MemberId,
    string? MemberName,
    Guid GalleryCategoryId,
    string? GalleryCategoryName,
    string Occasion,
    DateOnly CelebrationDate,
    Guid? GalleryImageId,
    string? PhotoUrl,
    bool PhotoPublicationConsent,
    string Status,
    DateTime? InvitationSentAt,
    DateTime? RespondedAt,
    DateTime? ApprovedByAdminAt,
    DateTime? PublishedAt,
    DateTime? WithdrawnAt)
{
    /// <summary>
    /// Creates a celebration DTO from the supplied celebration entity.
    /// </summary>
    /// <param name="celebration">
    /// The celebration entity to map.
    /// </param>
    /// <returns>
    /// A DTO containing celebration details, member information,
    /// selected gallery image, consent status and publication history.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the supplied celebration is null.
    /// </exception>
    /// <remarks>
    /// MemberName and GalleryCategoryName are populated when
    /// the corresponding navigation properties have been loaded.
    ///
    /// GalleryImageId identifies a photograph managed through
    /// the existing gallery infrastructure.
    ///
    /// PhotoUrl is retained for compatibility with existing
    /// celebration records.
    ///
    /// This DTO is intended for administrative operations.
    /// Public-facing endpoints should use a separate DTO
    /// exposing only information approved for publication.
    /// </remarks>
    public static CelebrationDto FromEntity(Celebration celebration)
    {
        ArgumentNullException.ThrowIfNull(celebration);

        return new CelebrationDto(
            Id: celebration.Id,
            MemberId: celebration.MemberId,

            MemberName: celebration.Member is null
                ? null
                : $"{celebration.Member.FirstName} {celebration.Member.LastName}".Trim(),

            GalleryCategoryId: celebration.GalleryCategoryId,
            GalleryCategoryName: celebration.GalleryCategory?.Name,
            Occasion: celebration.Occasion,
            CelebrationDate: celebration.CelebrationDate,
            GalleryImageId: celebration.GalleryImageId,
            PhotoUrl: celebration.PhotoUrl,
            PhotoPublicationConsent: celebration.PhotoPublicationConsent,
            Status: celebration.Status,
            InvitationSentAt: celebration.InvitationSentAt,
            RespondedAt: celebration.RespondedAt,
            ApprovedByAdminAt: celebration.ApprovedByAdminAt,
            PublishedAt: celebration.PublishedAt,
            WithdrawnAt: celebration.WithdrawnAt);
    }
}
