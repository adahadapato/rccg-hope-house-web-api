using FluentValidation;
using RccgHopeHouse.Application.Features.PastorPosts.Commands;

namespace RccgHopeHouse.Application.Features.PastorPosts.Validators;

/// <summary>
/// Validates UpdatePastorPostCommand.
/// Reuses create rules plus ID validation.
/// </summary>
public class UpdatePastorPostCommandValidator : AbstractValidator<UpdatePastorPostCommand>
{
    public UpdatePastorPostCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Post ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200);

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content cannot be empty.")
            .MaximumLength(50000);

        RuleFor(x => x.Category)
            .IsInEnum();

        RuleFor(x => x.CoverImageData)
            .Must(BeValidImageSize).When(x => x.CoverImageData is not null)
            .WithMessage("Cover image cannot exceed 5MB.");
    }

    private bool BeValidImageSize(byte[] imageData) => imageData.Length <= 5 * 1024 * 1024;
}