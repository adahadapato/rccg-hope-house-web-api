using MediatR;
using RccgHopeHouse.Application.Features.BirthdayWishes.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.BirthdayWishes.Commands;

/// <summary>
/// An admin approves (shows) or rejects (hides) a wish.
/// Works on any wish, so an earlier AI decision can be overridden.
/// </summary>
public record SetBirthdayWishStatusCommand(
    Guid Id,
    BirthdayWishStatus Status,
    string? Reason = null)
    : IRequest<AdminBirthdayWishDto>;

public class SetBirthdayWishStatusCommandHandler
    : IRequestHandler<SetBirthdayWishStatusCommand, AdminBirthdayWishDto>
{
    private readonly IGalleryImageCommentRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public SetBirthdayWishStatusCommandHandler(
        IGalleryImageCommentRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<AdminBirthdayWishDto> Handle(
        SetBirthdayWishStatusCommand request,
        CancellationToken ct)
    {
        if (request.Status is not (BirthdayWishStatus.Approved or BirthdayWishStatus.Rejected))
        {
            throw new DomainException("A wish can only be approved or rejected.");
        }

        var wish =
            await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(GalleryImageComment), request.Id);

        var reason = string.IsNullOrWhiteSpace(request.Reason)
            ? request.Status == BirthdayWishStatus.Approved
                ? "Approved by an admin."
                : "Rejected by an admin."
            : request.Reason;

        wish.ApplyDecision(
            request.Status,
            BirthdayWishDecisionSource.Admin,
            reason,
            _currentUser.UserName);

        await _repository.SaveChangesAsync(ct);

        return wish.ToAdminDto();
    }
}

/// <summary>An admin permanently deletes a wish.</summary>
public record DeleteBirthdayWishCommand(Guid Id) : IRequest;

public class DeleteBirthdayWishCommandHandler
    : IRequestHandler<DeleteBirthdayWishCommand>
{
    private readonly IGalleryImageCommentRepository _repository;

    public DeleteBirthdayWishCommandHandler(
        IGalleryImageCommentRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        DeleteBirthdayWishCommand request,
        CancellationToken ct)
    {
        var wish =
            await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(GalleryImageComment), request.Id);

        await _repository.DeleteAsync(wish, ct);
        await _repository.SaveChangesAsync(ct);
    }
}
