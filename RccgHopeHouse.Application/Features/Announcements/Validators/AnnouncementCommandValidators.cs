using FluentValidation;
using RccgHopeHouse.Application.Features.Announcements.Commands;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.Announcements;

/// <summary>
/// Fields shared by the create and update commands so both
/// can use the same validation rules.
/// </summary>
public interface IAnnouncementFields
{
    string Title { get; }
    AnnouncementType Type { get; }
    DateTime? StartsAt { get; }
    DateTime? EndsAt { get; }
    string? Summary { get; }
    string? Body { get; }
    string? LinkUrl { get; }
    string? LinkText { get; }
    int DisplayOrder { get; }
    DateTime? EventStartsAt { get; }
    DateTime? EventEndsAt { get; }
    string? VenueName { get; }
    string? VenueAddress { get; }
}

public abstract class AnnouncementFieldsValidator<T>
    : AbstractValidator<T>
    where T : IAnnouncementFields
{
    protected AnnouncementFieldsValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(Announcement.TitleMaxLength).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid announcement type.");

        RuleFor(x => x.Summary)
            .MaximumLength(Announcement.SummaryMaxLength).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Body)
            .MaximumLength(Announcement.BodyMaxLength).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.LinkUrl)
            .MaximumLength(Announcement.LinkUrlMaxLength).WithMessage(ValidationMessages.MaxLength)
            .Must(BeAValidLink)
            .When(x => !string.IsNullOrWhiteSpace(x.LinkUrl))
            .WithMessage("Link must be a full http(s) address, or a page anchor such as #events.");

        RuleFor(x => x.LinkText)
            .MaximumLength(Announcement.LinkTextMaxLength).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.EndsAt)
            .GreaterThanOrEqualTo(x => x.StartsAt!.Value)
            .When(x => x.EndsAt.HasValue && x.StartsAt.HasValue)
            .WithMessage("End date cannot be earlier than the start date.");

        RuleFor(x => x.EventStartsAt)
            .NotNull()
            .When(x => x.EventEndsAt.HasValue)
            .WithMessage("Add the event start time as well as the end time.");

        RuleFor(x => x.EventEndsAt)
            .GreaterThanOrEqualTo(x => x.EventStartsAt!.Value)
            .When(x => x.EventEndsAt.HasValue && x.EventStartsAt.HasValue)
            .WithMessage("The event cannot end before it starts.");

        RuleFor(x => x.VenueName)
            .MaximumLength(Announcement.VenueNameMaxLength).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.VenueAddress)
            .MaximumLength(Announcement.VenueAddressMaxLength).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be 0 or greater.");
    }

    private static bool BeAValidLink(string? value)
    {
        var link = value!.Trim();

        if (link.StartsWith('#') || link.StartsWith('/'))
        {
            return !link.StartsWith("//");
        }

        return Uri.TryCreate(link, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);
    }
}

public class CreateAnnouncementCommandValidator
    : AnnouncementFieldsValidator<CreateAnnouncementCommand>
{
}

public class UpdateAnnouncementCommandValidator
    : AnnouncementFieldsValidator<UpdateAnnouncementCommand>
{
}
