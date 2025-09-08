using FluentValidation;

namespace Identity.Application.Users.Validators;

public sealed class LoginUserValidator : AbstractValidator<Identity.Application.Users.Commands.LoginUser.Command>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}