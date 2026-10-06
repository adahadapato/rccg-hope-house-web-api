using MediatR;
using RccgHopeHouse.Application.Features.Members.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Application.Features.Members.Commands;

/// <summary>
/// Handles the creation of a new church member.
/// </summary>
public class CreateMemberCommandHandler
    : IRequestHandler<CreateMemberCommand, MemberDto>
{
    private readonly IMemberRepository _repository;
    private readonly IGalleryRepository _galleryRepository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="CreateMemberCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// The member repository.
    /// </param>
    /// <param name="galleryRepository">
    /// The gallery repository used to validate selected
    /// member photographs.
    /// </param>
    public CreateMemberCommandHandler(
        IMemberRepository repository,
        IGalleryRepository galleryRepository)
    {
        _repository = repository;
        _galleryRepository = galleryRepository;
    }

    /// <summary>
    /// Creates a new member from the supplied command.
    /// </summary>
    /// <param name="request">
    /// The member creation request.
    /// </param>
    /// <param name="ct">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// The newly created member.
    /// </returns>
    public async Task<MemberDto> Handle(
        CreateMemberCommand request,
        CancellationToken ct)
    {
        var address =
            CreateAddress(
                request);

        var birthday =
            CreateBirthday(
                request);

        var photo =
            await GetPhotoAsync(
                request.PhotoId,
                ct);

        var member =
            Member.Create(
                firstName:
                    request.FirstName,
                lastName:
                    request.LastName,
                email:
                    request.Email,
                phoneNumber:
                    request.PhoneNumber,
                address:
                    address,
                birthday:
                    birthday,
                maritalStatus:
                    request.MaritalStatus,
                weddingAnniversary:
                    request.WeddingAnniversary,
                consentToContact:
                    request.ConsentToContact,
                consentToBirthdayPublication:
                    request.ConsentToBirthdayPublication,
                joinedDate:
                    request.JoinedDate);

        if (photo is not null)
        {
            member.SetPhoto(
                photo.Id);
        }

        await _repository.AddAsync(
            member,
            ct);

        await _repository.SaveChangesAsync(
            ct);

        /*
         * Set the already validated gallery entity on the navigation
         * through the tracked relationship where possible by reloading
         * the newly created member. This also ensures the returned DTO
         * contains the photo paths.
         */
        var createdMember =
            await _repository.GetByIdAsync(
                member.Id,
                ct);

        return MemberDto.FromEntity(
            createdMember ?? member);
    }

    /// <summary>
    /// Gets and validates the selected gallery image.
    /// </summary>
    /// <param name="photoId">
    /// The selected gallery image identifier.
    /// </param>
    /// <param name="ct">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// The selected gallery image, or null when no image was selected.
    /// </returns>
    private async Task<GalleryImage?> GetPhotoAsync(
        Guid? photoId,
        CancellationToken ct)
    {
        if (!photoId.HasValue)
        {
            return null;
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

        return photo;
    }

    /// <summary>
    /// Creates an address value object when address information
    /// has been supplied. Returns null when all address fields are empty.
    /// </summary>
    /// <param name="request">
    /// The member creation request.
    /// </param>
    /// <returns>
    /// The address value object, or null when no address was supplied.
    /// </returns>
    private static Address? CreateAddress(
        CreateMemberCommand request)
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
    /// has been supplied. Returns null when no birthday is supplied.
    /// </summary>
    /// <param name="request">
    /// The member creation request.
    /// </param>
    /// <returns>
    /// The birthday value object, or null when no birthday was supplied.
    /// </returns>
    private static Birthday? CreateBirthday(
        CreateMemberCommand request)
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