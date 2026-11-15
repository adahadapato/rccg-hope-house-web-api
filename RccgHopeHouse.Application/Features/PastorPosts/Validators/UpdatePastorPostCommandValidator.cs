using FluentValidation;
using RccgHopeHouse.Application.Features.PastorPosts.Commands;

namespace RccgHopeHouse.Application.Features.PastorPosts.Validators;

/// <summary>
/// Validates UpdatePastorPostCommand.
/// Reuses create rules plus ID validation.
/// </summary>
public class UpdatePastorPostCommandValidator : AbstractValidator<UpdatePastorPostCommand>
{
    private const int MaxImageSizeBytes = 5 * 1024 * 1024; // 5MB

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

        RuleFor(x => x.ThemeOfTheYearId)
            .NotEmpty().WithMessage("A theme is required — every article must belong to a year's theme.");

        RuleFor(x => x.CoverImageData)
            .Must(BeValidImageSize)
            .WithMessage($"Cover image cannot exceed {MaxImageSizeBytes / 1024 / 1024}MB.");
    }

    private static bool BeValidImageSize(byte[]? imageData) =>
        imageData is null || imageData.Length <= MaxImageSizeBytes;
}