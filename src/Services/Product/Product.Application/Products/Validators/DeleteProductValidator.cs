using FluentValidation;

namespace Product.Application.Products.Validators;


public sealed class DeleteProductValidator : AbstractValidator<Product.Application.Products.Commands.DeleteProduct.Command>
{
    public DeleteProductValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}