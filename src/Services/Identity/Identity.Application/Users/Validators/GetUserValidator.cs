using FluentValidation;

namespace Identity.Application.Users.Validators;

public sealed class GetUserValidator : AbstractValidator<Identity.Application.Users.Commands.GetUser.Command>
{
    public GetUserValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}