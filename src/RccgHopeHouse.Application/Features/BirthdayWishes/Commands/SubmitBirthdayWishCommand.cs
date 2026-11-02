using System.Text.RegularExpressions;
using MediatR;
using Microsoft.Extensions.Logging;
using RccgHopeHouse.Application.Features.BirthdayWishes.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Interfaces.AI;
using RccgHopeHouse.Core.Results.AI;

namespace RccgHopeHouse.Application.Features.BirthdayWishes.Commands;

/// <summary>
/// A visitor sends a birthday wish on a celebrant photo.
/// </summary>
public record SubmitBirthdayWishCommand(
    Guid GalleryImageId,
    string AuthorName,
    string Message)
    : IRequest<SubmitBirthdayWishResultDto>;

/// <summary>
/// Saves the wish and decides whether it can be shown.
///
/// Order of checks:
/// 1. "Hold everything" setting on → hold for an admin.
/// 2. Links, emails or phone numbers → hold for an admin
///    (no AI call needed; these are the most common spam).
/// 3. AI check → publish, reject or hold.
/// 4. AI unavailable or any error → hold. A wish is never
///    published without a check.
/// </summary>
public partial class SubmitBirthdayWishCommandHandler
    : IRequestHandler<SubmitBirthdayWishCommand, SubmitBirthdayWishResultDto>
{
    private const string PublishedMessage =
        "Thank you! Your birthday wish is now showing.";

    private const string PendingMessage =
        "Thank you! Your wish will appear once it has been checked.";

    private const string RejectedMessage =
        "Sorry, we can't show this wish. Please keep wishes short, kind and free of links or contact details.";

    private readonly IGalleryImageCommentRepository _repository;
    private readonly ICommentModerationService _moderation;
    private readonly ILogger<SubmitBirthdayWishCommandHandler> _logger;

    public SubmitBirthdayWishCommandHandler(
        IGalleryImageCommentRepository repository,
        ICommentModerationService moderation,
        ILogger<SubmitBirthdayWishCommandHandler> logger)
    {
        _repository = repository;
        _moderation = moderation;
        _logger = logger;
    }

    public async Task<SubmitBirthdayWishResultDto> Handle(
        SubmitBirthdayWishCommand request,
        CancellationToken ct)
    {
        if (!await _repository.IsPublicCelebrantImageAsync(request.GalleryImageId, ct))
        {
            throw new NotFoundException("Celebrant photo", request.GalleryImageId);
        }

        var wish = GalleryImageComment.Create(
            request.GalleryImageId,
            request.AuthorName,
            request.Message);

        var (status, source, reason) = await DecideAsync(wish, ct);

        wish.ApplyDecision(status, source, reason);

        await _repository.AddAsync(wish, ct);
        await _repository.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Birthday wish {WishId} on image {ImageId} saved as {Status} by {Source}.",
            wish.Id,
            wish.GalleryImageId,
            status,
            source);

        return status switch
        {
            BirthdayWishStatus.Approved =>
                new SubmitBirthdayWishResultDto(status, PublishedMessage, wish.ToDto()),

            BirthdayWishStatus.Rejected =>
                new SubmitBirthdayWishResultDto(status, RejectedMessage, null),

            _ =>
                new SubmitBirthdayWishResultDto(status, PendingMessage, null)
        };
    }

    private async Task<(BirthdayWishStatus Status, BirthdayWishDecisionSource Source, string Reason)>
        DecideAsync(GalleryImageComment wish, CancellationToken ct)
    {
        if (_moderation.HoldAllForReview)
        {
            return (
                BirthdayWishStatus.Pending,
                BirthdayWishDecisionSource.System,
                "Held: the \"hold every wish for review\" setting is on.");
        }

        var combined = $"{wish.AuthorName}\n{wish.Message}";

        if (ContainsContactDetails(combined))
        {
            return (
                BirthdayWishStatus.Pending,
                BirthdayWishDecisionSource.System,
                "Held: contains a link, email address or phone number.");
        }

        try
        {
            var result = await _moderation.ModerateAsync(
                wish.AuthorName,
                wish.Message,
                ct);

            var status = result.Verdict switch
            {
                CommentModerationVerdict.Approve => BirthdayWishStatus.Approved,
                CommentModerationVerdict.Reject => BirthdayWishStatus.Rejected,
                _ => BirthdayWishStatus.Pending
            };

            return (status, BirthdayWishDecisionSource.Ai, result.Reason);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            // Never publish without a check: hold for an admin instead.
            _logger.LogWarning(
                ex,
                "Birthday wish AI check unavailable; holding wish for review.");

            return (
                BirthdayWishStatus.Pending,
                BirthdayWishDecisionSource.System,
                "Held: the automatic check was unavailable.");
        }
    }

    private static bool ContainsContactDetails(string text) =>
        LinkPattern().IsMatch(text) ||
        EmailPattern().IsMatch(text) ||
        PhonePattern().IsMatch(text);

    // http(s) links, www. addresses and bare domains such as example.com/abc
    [GeneratedRegex(
        @"(https?://|www\.|\b[a-z0-9-]+\.(com|net|org|uk|io|info|biz|xyz|ly|online|shop)\b)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex LinkPattern();

    [GeneratedRegex(
        @"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex EmailPattern();

    // 9 or more digits, allowing spaces, dashes, dots, brackets and a leading +
    [GeneratedRegex(
        @"\+?\d[\d\s().-]{7,}\d",
        RegexOptions.CultureInvariant)]
    private static partial Regex PhonePattern();
}
