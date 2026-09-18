using FluentValidation;
using RccgHopeHouse.Application.Features.PastorPosts.Commands;

namespace RccgHopeHouse.Application.Features.PastorPosts.Validators;

/// <summary>
/// Validates CreatePastorPostCommand before handler execution.
/// Ensures data integrity and prevents invalid payloads from reaching the domain.
/// </summary>
public class CreatePastorPostCommandValidator : AbstractValidator<CreatePastorPostCommand>
{
    private const int MaxImageSizeBytes = 5 * 1024 * 1024; // 5MB

    public CreatePastorPostCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content cannot be empty.")
            .MaximumLength(50000).WithMessage("Content cannot exceed 50,000 characters.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid post category.");

        RuleFor(x => x.ThemeOfTheYearId)
            .NotEmpty().WithMessage("A theme is required — every article must belong to a year's theme.");

        RuleFor(x => x.AuthorName)
            .NotEmpty().WithMessage("Author name is required.")
            .MaximumLength(100).WithMessage("Author name cannot exceed 100 characters.");

        RuleFor(x => x.CoverImageData)
            .Must(BeValidImageSize)
            .WithMessage($"Cover image cannot exceed {MaxImageSizeBytes / 1024 / 1024}MB.");

        RuleFor(x => x.BibleReference)
            .MaximumLength(50).WithMessage("Bible reference cannot exceed 50 characters.");
    }

    /// <summary>
    /// Validates image size to prevent database bloat. Null is always valid (no image provided).
    /// </summary>
    private static bool BeValidImageSize(byte[]? imageData) =>
        imageData is null || imageData.Length <= MaxImageSizeBytes;
}