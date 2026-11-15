
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Handles updates to existing pending celebrations.
/// </summary>
/// <remarks>
/// Ensures that the celebration exists and remains pending,
/// validates its member and gallery category, confirms the
/// date matches the member's recorded occasion, and prevents
/// duplicate celebration records.
///
/// Changes are persisted using ICelebrationRepository.
/// </remarks>
public sealed class UpdateCelebrationCommandHandler
    : IRequestHandler<UpdateCelebrationCommand, CelebrationDto>
{
    private readonly ICelebrationRepository _celebrationRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IGalleryCategoryRepository _galleryCategoryRepository;

    /// <summary>
    /// Initializes the update celebration handler.
    /// </summary>
    /// <param name="celebrationRepository">
    /// Repository responsible for celebration persistence.
    /// </param>
    /// <param name="memberRepository">
    /// Repository responsible for member information.
    /// </param>
    /// <param name="galleryCategoryRepository">
    /// Repository responsible for gallery categories.
    /// </param>
    public UpdateCelebrationCommandHandler(
        ICelebrationRepository celebrationRepository,
        IMemberRepository memberRepository,
        IGalleryCategoryRepository galleryCategoryRepository)
    {
        _celebrationRepository = celebrationRepository
            ?? throw new ArgumentNullException(
                nameof(celebrationRepository));

        _memberRepository = memberRepository
            ?? throw new ArgumentNullException(
                nameof(memberRepository));

        _galleryCategoryRepository = galleryCategoryRepository
            ?? throw new ArgumentNullException(
                nameof(galleryCategoryRepository));
    }

    /// <summary>
    /// Validates and updates an existing celebration.
    /// </summary>
    /// <param name="request">
    /// The celebration update command.
    /// </param>
    /// <param name="ct">
    /// Cancellation token for asynchronous operations.
    /// </param>
    /// <returns>
    /// The updated celebration DTO.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the celebration, member or gallery
    /// category does not exist.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the celebration cannot be modified
    /// or a duplicate celebration exists.
    /// </exception>
    public async Task<CelebrationDto> Handle(
        UpdateCelebrationCommand request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Id == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid celebration ID is required.",
                nameof(request.Id));
        }

        if (request.GalleryCategoryId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid gallery category ID is required.",
                nameof(request.GalleryCategoryId));
        }

        if (request.Occasion is not ("Birthday" or "WeddingAnniversary"))
        {
            throw new ArgumentException(
                "Occasion must be Birthday or WeddingAnniversary.",
                nameof(request.Occasion));
        }

        if (request.CelebrationDate == default)
        {
            throw new ArgumentException(
                "A valid celebration date is required.",
                nameof(request.CelebrationDate));
        }

        // Retrieve the existing celebration.
        var celebration =
            await _celebrationRepository.GetByIdAsync(
                request.Id,
                ct);

        if (celebration is null)
        {
            throw new KeyNotFoundException(
                "The selected celebration does not exist.");
        }

        // Prevent changes after member response
        // or administrator approval.
        if (celebration.Status != "Pending")
        {
            throw new InvalidOperationException(
                "Only pending celebrations can be updated.");
        }

        // Retrieve and validate the associated member.
        var member = await _memberRepository.GetByIdAsync(
            celebration.MemberId,
            ct);

        if (member is null)
        {
            throw new KeyNotFoundException(
                "The associated church member does not exist.");
        }

        if (!member.IsActive)
        {
            throw new InvalidOperationException(
                "Celebrations cannot be updated for inactive members.");
        }

        if (!member.ConsentToContact)
        {
            throw new InvalidOperationException(
                "The member has not consented to being contacted.");
        }

        // Validate the selected gallery category.
        var galleryCategory =
            await _galleryCategoryRepository.GetByIdAsync(
                request.GalleryCategoryId,
                ct);

        if (galleryCategory is null)
        {
            throw new KeyNotFoundException(
                "The selected gallery category does not exist.");
        }

        // Confirm the updated date matches member records.
        ValidateCelebrationDate(
            member,
            request.Occasion,
            request.CelebrationDate);

        // Check for another celebration with the same
        // member, occasion and date.
        var duplicate =
            await _celebrationRepository
                .GetByMemberOccasionAndDateAsync(
                    celebration.MemberId,
                    request.Occasion,
                    request.CelebrationDate,
                    ct);

        if (duplicate is not null &&
            duplicate.Id != celebration.Id)
        {
            throw new InvalidOperationException(
                "Another celebration already exists for this member, occasion, and date.");
        }

        // Apply the changes through the domain entity.
        celebration.UpdateDetails(
            request.GalleryCategoryId,
            request.Occasion,
            request.CelebrationDate);

        // Persist the updated celebration.
        await _celebrationRepository.UpdateAsync(
            celebration,
            ct);

        await _celebrationRepository.SaveChangesAsync(ct);

        // Reload the celebration and its relationships.
        var updatedCelebration =
            await _celebrationRepository.GetByIdAsync(
                celebration.Id,
                ct);

        return CelebrationDto.FromEntity(
            updatedCelebration ?? celebration);
    }

    /// <summary>
    /// Validates that the updated date matches the
    /// member's birthday or wedding anniversary.
    /// </summary>
    /// <param name="member">
    /// The associated church member.
    /// </param>
    /// <param name="occasion">
    /// The requested celebration occasion.
    /// </param>
    /// <param name="celebrationDate">
    /// The proposed celebration date.
    /// </param>
    private static void ValidateCelebrationDate(
        Member member,
        string occasion,
        DateOnly celebrationDate)
    {
        if (occasion == "Birthday")
        {
            if (member.Birthday is null)
            {
                throw new InvalidOperationException(
                    "The member does not have a recorded birthday.");
            }

            if (member.Birthday.Month != celebrationDate.Month ||
                member.Birthday.Day != celebrationDate.Day)
            {
                throw new ArgumentException(
                    "The celebration date does not match the member's recorded birthday.",
                    nameof(celebrationDate));
            }

            return;
        }

        if (occasion == "WeddingAnniversary")
        {
            if (!member.WeddingAnniversary.HasValue)
            {
                throw new InvalidOperationException(
                    "The member does not have a recorded wedding anniversary.");
            }

            var anniversary = member.WeddingAnniversary.Value;

            if (anniversary.Month != celebrationDate.Month ||
                anniversary.Day != celebrationDate.Day)
            {
                throw new ArgumentException(
                    "The celebration date does not match the member's recorded wedding anniversary.",
                    nameof(celebrationDate));
            }
        }
    }
}
