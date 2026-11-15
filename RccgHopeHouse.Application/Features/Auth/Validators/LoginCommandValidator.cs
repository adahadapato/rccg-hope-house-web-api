using FluentValidation;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.Auth.Commands;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .EmailAddress().WithMessage(ValidationMessages.InvalidEmail);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ValidationMessages.Required);
    }
}