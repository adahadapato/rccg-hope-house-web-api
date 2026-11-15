using MediatR;
using RccgHopeHouse.Application.Features.Members.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Application.Features.Members.Commands;

/// <summary>
/// Handles updates to an existing church member.
/// </summary>
public class UpdateMemberCommandHandler
    : IRequestHandler<UpdateMemberCommand, MemberDto>
{
    private readonly IMemberRepository _repository;
    private readonly IGalleryRepository _galleryRepository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="UpdateMemberCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// The member repository.
    /// </param>
    /// <param name="galleryRepository">
    /// The gallery repository used to validate selected
    /// member photographs.
    /// </param>
    public UpdateMemberCommandHandler(
        IMemberRepository repository,
        IGalleryRepository galleryRepository)
    {
        _repository = repository;
        _galleryRepository = galleryRepository;
    }

    /// <summary>
    /// Updates an existing member using the supplied command.
    /// </summary>
    /// <param name="request">
    /// The member update request.
    /// </param>
    /// <param name="ct">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// The updated member.
    /// </returns>
    public async Task<MemberDto> Handle(
        UpdateMemberCommand request,
        CancellationToken ct)
    {
        var member =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(Member),
                request.Id);

        var address =
            CreateAddress(
                request);

        var birthday =
            CreateBirthday(
                request);

        await ValidatePhotoAsync(
            request.PhotoId,
            ct);

        member.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber);

        member.UpdateAddress(
            address);

        member.UpdateBirthday(
            birthday);

        member.UpdateMaritalInfo(
            request.MaritalStatus,
            request.WeddingAnniversary);

        member.SetConsentToContact(
            request.ConsentToContact);

        member.SetBirthdayPublicationConsent(
            request.ConsentToBirthdayPublication);

        UpdatePhoto(
            member,
            request.PhotoId);

        // A null JoinedDate on update means:
        // preserve the member's existing joined date.
        if (request.JoinedDate.HasValue)
        {
            member.UpdateJoinedDate(
                request.JoinedDate.Value);
        }

        await _repository.UpdateAsync(
            member,
            ct);

        await _repository.SaveChangesAsync(
            ct);

        /*
         * Reload so the returned DTO contains the currently selected
         * GalleryImage navigation and therefore its image paths.
         */
        var updatedMember =
            await _repository.GetByIdAsync(
                member.Id,
                ct);

        return MemberDto.FromEntity(
            updatedMember ?? member);
    }

    /// <summary>
    /// Validates that a selected gallery image exists.
    /// </summary>
    /// <param name="photoId">
    /// The selected gallery image identifier.
    /// </param>
    /// <param name="ct">
    /// The cancellation token.
    /// </param>
    private async Task ValidatePhotoAsync(
        Guid? photoId,
        CancellationToken ct)
    {
        if (!photoId.HasValue)
        {
            return;
        }

        if (photoId.Value == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid gallery image ID is required.",
                nameof(photoId));
        }

        var photo =
            await _galleryRepository
                .GetImageByIdAsync(
                    photoId.Value,
                    includeTags: false,
                    ct: ct);

        if (photo is null)
        {
            throw new ArgumentException(
                "The selected member photo does not exist.",
                nameof(photoId));
        }
    }

    /// <summary>
    /// Updates the gallery image assigned as the member's
    /// birthday/profile photograph.
    /// </summary>
    /// <param name="member">
    /// The member being updated.
    /// </param>
    /// <param name="photoId">
    /// The gallery image identifier, or null to remove the
    /// currently selected photograph.
    /// </param>
    private static void UpdatePhoto(
        Member member,
        Guid? photoId)
    {
        if (photoId.HasValue)
        {
            member.SetPhoto(
                photoId.Value);

            return;
        }

        member.RemovePhoto();
    }

    /// <summary>
    /// Creates an address value object when address information
    /// has been supplied. Returns null when all address fields are empty,
    /// which removes the member's existing address.
    /// </summary>
    /// <param name="request">
    /// The member update request.
    /// </param>
    /// <returns>
    /// The address value object, or null when no address was supplied.
    /// </returns>
    private static Address? CreateAddress(
        UpdateMemberCommand request)
    {
        var hasAddress =
            !string.IsNullOrWhiteSpace(
                request.AddressLine1) ||
            !string.IsNullOrWhiteSpace(
                request.AddressLine2) ||
            !string.IsNullOrWhiteSpace(
                request.City) ||
            !string.IsNullOrWhiteSpace(
                request.County) ||
            !string.IsNullOrWhiteSpace(
                request.Postcode) ||
            !string.IsNullOrWhiteSpace(
                request.Country);

        if (!hasAddress)
        {
            return null;
        }

        return Address.Create(
            addressLine1:
                request.AddressLine1 ??
                string.Empty,
            addressLine2:
                request.AddressLine2,
            city:
                request.City ??
                string.Empty,
            county:
                request.County,
            postcode:
                request.Postcode ??
                string.Empty,
            country:
                request.Country ??
                string.Empty);
    }

    /// <summary>
    /// Creates a birthday value object when birthday information
    /// has been supplied. Returns null when all birthday fields are empty,
    /// which removes the member's existing birthday.
    /// </summary>
    /// <param name="request">
    /// The member update request.
    /// </param>
    /// <returns>
    /// The birthday value object, or null when no birthday was supplied.
    /// </returns>
    private static Birthday? CreateBirthday(
        UpdateMemberCommand request)
    {
        var hasBirthday =
            request.BirthMonth.HasValue ||
            request.BirthDay.HasValue ||
            request.BirthYear.HasValue;

        if (!hasBirthday)
        {
            return null;
        }

        if (!request.BirthMonth.HasValue ||
            !request.BirthDay.HasValue)
        {
            throw new ArgumentException(
                "Birth month and day must both be provided when a birthday is supplied.");
        }

        return Birthday.Create(
            month:
                request.BirthMonth.Value,
            day:
                request.BirthDay.Value,
            year:
                request.BirthYear);
    }
}