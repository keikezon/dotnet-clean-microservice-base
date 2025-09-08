using FluentValidation;

namespace Product.Application.Products.Validators;

public class UpdateStockValidator : AbstractValidator<Product.Application.Products.Commands.UpdateStock.Command>
{
    public UpdateStockValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Quantity).NotEmpty();
        RuleFor(x => x.Invoice).NotEmpty();
    }
}