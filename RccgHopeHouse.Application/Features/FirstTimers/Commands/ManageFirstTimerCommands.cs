using MediatR;
using RccgHopeHouse.Application.Features.FirstTimers.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Application.Features.FirstTimers.Commands;

// ===== Follow-up =====

public record UpdateFirstTimerFollowUpCommand(
    Guid Id,
    FirstTimerFollowUpStatus Status,
    string? Note)
    : IRequest<FirstTimerDto>;

public sealed class UpdateFirstTimerFollowUpCommandHandler
    : IRequestHandler<UpdateFirstTimerFollowUpCommand, FirstTimerDto>
{
    private readonly IFirstTimerRepository _repository;

    public UpdateFirstTimerFollowUpCommandHandler(IFirstTimerRepository repository)
    {
        _repository = repository;
    }

    public async Task<FirstTimerDto> Handle(
        UpdateFirstTimerFollowUpCommand request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var firstTimer =
            await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(FirstTimer), request.Id);

        firstTimer.UpdateFollowUp(request.Status, request.Note);

        await _repository.SaveChangesAsync(ct);

        return FirstTimerDto.FromEntity(firstTimer);
    }
}

// ===== Add to members =====

/// <param name="Id">The first-timer to promote.</param>
/// <param name="Force">
/// Create the member even though one with the same phone number
/// or email already exists.
/// </param>
public record PromoteFirstTimerCommand(Guid Id, bool Force = false)
    : IRequest<PromoteFirstTimerResultDto>;

public sealed class PromoteFirstTimerCommandHandler
    : IRequestHandler<PromoteFirstTimerCommand, PromoteFirstTimerResultDto>
{
    private const string DefaultCountry = "United Kingdom";

    private readonly IFirstTimerRepository _repository;
    private readonly IMemberRepository _memberRepository;

    public PromoteFirstTimerCommandHandler(
        IFirstTimerRepository repository,
        IMemberRepository memberRepository)
    {
        _repository = repository;
        _memberRepository = memberRepository;
    }

    public async Task<PromoteFirstTimerResultDto> Handle(
        PromoteFirstTimerCommand request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var firstTimer =
            await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(FirstTimer), request.Id);

        if (firstTimer.MemberId.HasValue)
        {
            return new PromoteFirstTimerResultDto(
                Promoted: false,
                MemberId: firstTimer.MemberId,
                Message: "This visitor has already been added to the members list.");
        }

        if (!request.Force)
        {
            var duplicate = await FindMatchingMemberAsync(firstTimer, ct);

            if (duplicate is not null)
            {
                var duplicateName = $"{duplicate.FirstName} {duplicate.LastName}";

                return new PromoteFirstTimerResultDto(
                    Promoted: false,
                    MemberId: null,
                    Message:
                        $"A member with the same phone number or email already exists ({duplicateName}).",
                    PossibleDuplicateMemberId: duplicate.Id,
                    PossibleDuplicateMemberName: duplicateName);
            }
        }

        var member = Member.Create(
            firstName: firstTimer.FirstName,
            lastName: firstTimer.LastName,
            email: firstTimer.Email,
            phoneNumber: firstTimer.PhoneNumber,
            address: CreateAddress(firstTimer),
            birthday: CreateBirthday(firstTimer),
            consentToContact: firstTimer.ConsentToContact);

        await _memberRepository.AddAsync(member, ct);

        firstTimer.MarkPromoted(member.Id);

        // Both repositories share one database context, so the new
        // member and the link to it are saved together.
        await _repository.SaveChangesAsync(ct);

        return new PromoteFirstTimerResultDto(
            Promoted: true,
            MemberId: member.Id,
            Message: $"{member.FirstName} {member.LastName} has been added to the members list.");
    }

    private async Task<Member?> FindMatchingMemberAsync(
        FirstTimer firstTimer,
        CancellationToken ct)
    {
        // The congregation is small, so comparing in memory is fine
        // and lets "07..." match "+44...".
        var members = await _memberRepository.GetAllAsync(
            includeInactive: true,
            skip: 0,
            take: int.MaxValue,
            ct);

        return members.FirstOrDefault(member =>
            (member.PhoneNumber is not null &&
             FirstTimer.NormalizePhone(member.PhoneNumber.Value) ==
                 firstTimer.NormalizedPhoneNumber) ||
            (member.Email is not null &&
             firstTimer.Email is not null &&
             string.Equals(
                 member.Email.Value,
                 firstTimer.Email,
                 StringComparison.OrdinalIgnoreCase)));
    }

    private static Address? CreateAddress(FirstTimer firstTimer)
    {
        if (string.IsNullOrWhiteSpace(firstTimer.AddressLine1) ||
            string.IsNullOrWhiteSpace(firstTimer.City) ||
            string.IsNullOrWhiteSpace(firstTimer.Postcode))
        {
            return null;
        }

        return Address.Create(
            addressLine1: firstTimer.AddressLine1,
            addressLine2: firstTimer.AddressLine2,
            city: firstTimer.City,
            county: null,
            postcode: firstTimer.Postcode,
            country: DefaultCountry);
    }

    private static Birthday? CreateBirthday(FirstTimer firstTimer)
    {
        if (!firstTimer.BirthMonth.HasValue || !firstTimer.BirthDay.HasValue)
        {
            return null;
        }

        return Birthday.Create(
            month: firstTimer.BirthMonth.Value,
            day: firstTimer.BirthDay.Value);
    }
}

// ===== Delete =====

public record DeleteFirstTimerCommand(Guid Id) : IRequest<Unit>;

public sealed class DeleteFirstTimerCommandHandler
    : IRequestHandler<DeleteFirstTimerCommand, Unit>
{
    private readonly IFirstTimerRepository _repository;

    public DeleteFirstTimerCommandHandler(IFirstTimerRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(
        DeleteFirstTimerCommand request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var firstTimer =
            await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(FirstTimer), request.Id);

        await _repository.DeleteAsync(firstTimer, ct);
        await _repository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
