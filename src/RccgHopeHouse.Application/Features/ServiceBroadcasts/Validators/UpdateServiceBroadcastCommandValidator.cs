using FluentValidation;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Validators;

public class UpdateServiceBroadcastCommandValidator
    : AbstractValidator<UpdateServiceBroadcastCommand>
{
    public UpdateServiceBroadcastCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.YoutubeUrl)
            .NotEmpty()
            .WithMessage("A YouTube URL or video ID is required.");
    }
}