using FluentValidation;

namespace Order.Application.Orders.Validators;

public class CreateOrderItemValidator : AbstractValidator<Order.Application.Orders.Commands.CreateOrder.Command>
{
    public CreateOrderItemValidator()
    {
        RuleFor(x => x.OrderModel.UserId).NotEmpty();
        RuleFor(x => x.OrderModel.Items).NotEmpty();
    }
}