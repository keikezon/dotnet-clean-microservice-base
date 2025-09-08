using FluentValidation;

namespace Product.Application.Products.Validators;

public class DecreaseStockValidator : AbstractValidator<Product.Application.Products.Commands.DecreaseStock.Command>
{
    public DecreaseStockValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Quantity).NotEmpty();
    }
}