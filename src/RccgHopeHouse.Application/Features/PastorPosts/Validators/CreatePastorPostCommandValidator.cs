using FluentValidation;
using RccgHopeHouse.Application.Features.PastorPosts.Commands;

namespace RccgHopeHouse.Application.Features.PastorPosts.Validators;

/// <summary>
/// Validates CreatePastorPostCommand before handler execution.
/// Ensures data integrity and prevents invalid payloads from reaching the domain.
/// </summary>
public class CreatePastorPostCommandValidator : AbstractValidator<CreatePastorPostCommand>
{
    /// <summary>
    /// Initializes validation rules.
    /// </summary>
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

        RuleFor(x => x.AuthorName)
            .NotEmpty().WithMessage("Author name is required.")
            .MaximumLength(100).WithMessage("Author name cannot exceed 100 characters.");

        RuleFor(x => x.CoverImageData)
            .Must(BeValidImageSize).When(x => x.CoverImageData is not null)
            .WithMessage("Cover image cannot exceed 5MB.");

        RuleFor(x => x.BibleReference)
            .MaximumLength(50).WithMessage("Bible reference cannot exceed 50 characters.");

        RuleFor(x => x.Theme)
            .MaximumLength(100).WithMessage("Theme cannot exceed 100 characters.");
    }

    /// <summary>
    /// Validates image size to prevent database bloat.
    /// </summary>
    private bool BeValidImageSize(byte[] imageData) => imageData.Length <= 5 * 1024 * 1024; // 5MB limit
}