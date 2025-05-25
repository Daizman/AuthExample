using AuthExample.Contracts;
using FluentValidation;

namespace AuthExample.Services.Validators;

public class SignUpValidator : AbstractValidator<SignUpDto>
{
    public SignUpValidator()
    {
        RuleFor(createDto => createDto.UserName)
            .NotNull()
            .NotEmpty()
            .MaximumLength(128);
        RuleFor(createDto => createDto.Password)
            .NotNull()
            .NotEmpty()
            .MaximumLength(256);
    }
}
