
using FluentValidation;
using RccgHopeHouse.Application.Features.Celebrations.Commands;

namespace RccgHopeHouse.Application.Features.Celebrations.Validators;

/// <summary>
/// Validates requests to soft-delete celebration records.
/// </summary>
/// <remarks>
/// Ensures the request contains a valid celebration
/// identifier.
///
/// Database-dependent checks, including existence,
/// publication status and administrator approval,
/// are performed by DeleteCelebrationCommandHandler.
/// </remarks>
public sealed class DeleteCelebrationCommandValidator
    : AbstractValidator<DeleteCelebrationCommand>
{
    /// <summary>
    /// Initializes the validation rules for deleting
    /// a celebration.
    /// </summary>
    public DeleteCelebrationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(
                "A valid celebration ID is required.");
    }
}
