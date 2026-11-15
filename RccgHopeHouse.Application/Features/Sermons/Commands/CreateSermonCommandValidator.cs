using FluentValidation;

namespace RccgHopeHouse.Application.Features.Sermons.Commands
{
    public class CreateSermonCommandValidator : AbstractValidator<CreateSermonCommand>
    {
        public CreateSermonCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Speaker).NotEmpty().MaximumLength(100);
            RuleFor(x => x.VideoUrl).NotEmpty().Must(BeValidYouTubeUrl).WithMessage("Invalid YouTube URL");
            RuleFor(x => x.ServiceDate).LessThanOrEqualTo(DateTime.UtcNow.AddDays(1));
        }

        private bool BeValidYouTubeUrl(string url) =>
            Uri.IsWellFormedUriString(url, UriKind.Absolute) &&
            (url.Contains("youtube.com") || url.Contains("youtu.be"));
    }
}
