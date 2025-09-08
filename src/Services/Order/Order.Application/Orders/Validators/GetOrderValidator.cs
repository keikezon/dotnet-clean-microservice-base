using FluentValidation;

namespace Order.Application.Orders.Validators;

public class GetOrderValidator : AbstractValidator<Order.Application.Orders.Commands.GetOrder.Command>
{
    public GetOrderValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}