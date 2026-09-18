using FluentValidation;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Validators;

public class CreateServiceBroadcastCommandValidator : AbstractValidator<CreateServiceBroadcastCommand>
{
    public CreateServiceBroadcastCommandValidator()
    {
        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.YoutubeUrl).NotEmpty().WithMessage("A YouTube URL or video ID is required.");
    }
}