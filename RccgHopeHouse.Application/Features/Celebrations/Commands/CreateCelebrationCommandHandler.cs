
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Handles the creation of birthday and wedding
/// anniversary celebrations for church members.
/// </summary>
/// <remarks>
/// Performs database-dependent validation, including
/// member existence, membership status, gallery category
/// existence, and duplicate celebration detection.
///
/// Ensures the celebration date matches the member's
/// recorded birthday or wedding anniversary.
///
/// All new celebrations begin with Pending status.
///
/// Member publication consent and administrator approval
/// are handled by separate commands.
/// </remarks>
public sealed class CreateCelebrationCommandHandler
    : IRequestHandler<CreateCelebrationCommand, CelebrationDto>
{
    private readonly ICelebrationRepository _celebrationRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IGalleryCategoryRepository _galleryCategoryRepository;

    /// <summary>
    /// Initializes the celebration creation handler.
    /// </summary>
    /// <param name="celebrationRepository">
    /// Repository responsible for celebration persistence.
    /// </param>
    /// <param name="memberRepository">
    /// Repository responsible for retrieving church members.
    /// </param>
    /// <param name="galleryCategoryRepository">
    /// Repository responsible for retrieving gallery categories.
    /// </param>
    public CreateCelebrationCommandHandler(
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
    /// Validates the supplied celebration information,
    /// creates a pending celebration, and saves it.
    /// </summary>
    /// <param name="request">
    /// The celebration creation command.
    /// </param>
    /// <param name="ct">
    /// Cancellation token for asynchronous operations.
    /// </param>
    /// <returns>
    /// A DTO representing the newly created celebration.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when identifiers, occasion, or date are invalid.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the member or gallery category is not found.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the member is inactive, contact consent
    /// is absent, or a duplicate celebration exists.
    /// </exception>
    public async Task<CelebrationDto> Handle(
        CreateCelebrationCommand request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Validate identifiers.
        if (request.MemberId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid member ID is required.",
                nameof(request.MemberId));
        }

        if (request.GalleryCategoryId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid gallery category ID is required.",
                nameof(request.GalleryCategoryId));
        }

        // Validate the supported occasion.
        if (request.Occasion is not ("Birthday" or "WeddingAnniversary"))
        {
            throw new ArgumentException(
                "Occasion must be Birthday or WeddingAnniversary.",
                nameof(request.Occasion));
        }

        // Validate the requested date.
        if (request.CelebrationDate == default)
        {
            throw new ArgumentException(
                "A valid celebration date is required.",
                nameof(request.CelebrationDate));
        }

        // Retrieve the member.
        var member = await _memberRepository.GetByIdAsync(
            request.MemberId,
            ct);

        if (member is null)
        {
            throw new KeyNotFoundException(
                "The selected church member does not exist.");
        }

        // Only active members can be scheduled.
        if (!member.IsActive)
        {
            throw new InvalidOperationException(
                "Celebrations cannot be created for inactive members.");
        }

        // Ensure the member has consented to contact.
        // This is separate from consent to publish.
        if (!member.ConsentToContact)
        {
            throw new InvalidOperationException(
                "The member has not consented to being contacted.");
        }

        // Retrieve the existing gallery category.
        var galleryCategory =
            await _galleryCategoryRepository.GetByIdAsync(
                request.GalleryCategoryId,
                ct);

        if (galleryCategory is null)
        {
            throw new KeyNotFoundException(
                "The selected gallery category does not exist.");
        }

        // Confirm that the celebration date matches
        // the member's recorded birthday or anniversary.
        ValidateCelebrationDate(
            member,
            request.Occasion,
            request.CelebrationDate);

        // Prevent duplicate celebrations.
        var existingCelebration =
            await _celebrationRepository
                .GetByMemberOccasionAndDateAsync(
                    request.MemberId,
                    request.Occasion,
                    request.CelebrationDate,
                    ct);

        if (existingCelebration is not null)
        {
            throw new InvalidOperationException(
                "A celebration already exists for this member, occasion, and date.");
        }

        // Create a new pending celebration.
        var celebration = Celebration.Create(
            memberId: request.MemberId,
            galleryCategoryId: request.GalleryCategoryId,
            occasion: request.Occasion,
            celebrationDate: request.CelebrationDate);

        // Track and persist the celebration.
        await _celebrationRepository.AddAsync(
            celebration,
            ct);

        await _celebrationRepository.SaveChangesAsync(ct);

        // Reload related entities for DTO mapping.
        var createdCelebration =
            await _celebrationRepository.GetByIdAsync(
                celebration.Id,
                ct);

        return CelebrationDto.FromEntity(
            createdCelebration ?? celebration);
    }

    /// <summary>
    /// Ensures the requested celebration date matches
    /// the member's recorded birthday or wedding anniversary.
    /// </summary>
    /// <param name="member">
    /// The church member being celebrated.
    /// </param>
    /// <param name="occasion">
    /// The celebration occasion.
    /// </param>
    /// <param name="celebrationDate">
    /// The proposed celebration date.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the required member information is missing.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the date does not match the member's records.
    /// </exception>
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
