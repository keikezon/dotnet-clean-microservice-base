using FluentValidation;

namespace Identity.Application.Users.Validators;

public sealed class CreateUserValidator : AbstractValidator<Identity.Application.Users.Commands.CreateUser.Command>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}
