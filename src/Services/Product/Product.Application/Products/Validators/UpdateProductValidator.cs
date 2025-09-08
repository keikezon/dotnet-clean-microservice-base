using FluentValidation;

namespace Product.Application.Products.Validators;

public class UpdateProductValidator : AbstractValidator<Product.Application.Products.Commands.UpdateProduct.Command>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Price).NotEmpty();
        RuleFor(x => x.Stock).NotEmpty();
    }
}