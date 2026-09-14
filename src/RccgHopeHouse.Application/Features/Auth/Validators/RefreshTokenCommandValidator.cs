using FluentValidation;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.Auth.Commands;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage(ValidationMessages.Required);
    }
}