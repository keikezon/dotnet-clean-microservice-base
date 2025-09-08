using MassTransit;
using Order.Application.Orders.Abstractions;
using Order.Contracts.Events;
using Order.Domain.Orders;

namespace Order.Application.Orders.Commands;

public sealed class CreateOrder
{
    public sealed record Command(OrderModel OrderModel);

    public interface IHandler
    {
        Task<OrderModel> Handle(Command cmd, CancellationToken ct);
    }

    public sealed class Handler(IOrderRepository repo, IPublishEndpoint bus) : IHandler
    {
        public async Task<OrderModel> Handle(Command cmd, CancellationToken ct)
        {
            var order = OrderModel.Create(cmd.OrderModel);
            await repo.AddAsync(order, ct);
            await repo.SaveChangesAsync(ct);

            // Publica evento via contrato
            var @event = new OrderCreatedIntegrationEvent(order.Id);
            await bus.Publish(@event, ct);

            var newOrder = await repo.GetByIdAsync(order.Id, ct);
            return newOrder;
        }
    }
}