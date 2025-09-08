using FluentValidation;

namespace Product.Application.Products.Validators;


public sealed class GetProductValidator : AbstractValidator<Product.Application.Products.Commands.GetProduct.Command>
{
    public GetProductValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}