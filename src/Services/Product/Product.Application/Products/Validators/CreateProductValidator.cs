using FluentValidation;

namespace Product.Application.Products.Validators;

public class CreateProductValidator : AbstractValidator<Product.Application.Products.Commands.CreateProduct.Command>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Price).NotEmpty();
        RuleFor(x => x.Stock).NotEmpty();
    }
}