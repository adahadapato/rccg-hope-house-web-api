using FluentValidation;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Validators;

public class CreateServiceBroadcastCommandValidator
    : AbstractValidator<CreateServiceBroadcastCommand>
{
    public CreateServiceBroadcastCommandValidator()
    {
        RuleFor(x => x.ChurchServiceId)
            .NotEmpty()
            .WithMessage(
                "A church service is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.YoutubeUrl)
            .NotEmpty()
            .WithMessage(
                "A YouTube URL or video ID is required.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x =>
                !string.IsNullOrWhiteSpace(
                    x.Description));

        RuleFor(x => x.Theme)
            .MaximumLength(100)
            .When(x =>
                !string.IsNullOrWhiteSpace(
                    x.Theme));

        RuleFor(x => x.ServiceMonth)
            .NotEmpty()
            .WithMessage(
                "Service month is required.");
    }
}