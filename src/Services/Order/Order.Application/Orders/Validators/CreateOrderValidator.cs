using FluentValidation;

namespace Order.Application.Orders.Validators;

public class CreateOrderValidator : AbstractValidator<Order.Application.Orders.Commands.CreateOrder.Command>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.OrderModel.UserId).NotEmpty();
        RuleFor(x => x.OrderModel.ClientDocument).NotEmpty();
        RuleFor(x => x.OrderModel.Items).NotEmpty().WithMessage("Items is required");
        RuleForEach(x => x.OrderModel.Items)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty().WithMessage("Product Id is required");
                item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
            });
    }
}