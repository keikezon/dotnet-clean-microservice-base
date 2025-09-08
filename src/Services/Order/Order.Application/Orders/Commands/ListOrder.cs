using Order.Application.Orders.Abstractions;
using Order.Domain.Orders;

namespace Order.Application.Orders.Commands;

public sealed class ListOrder
{
    public sealed record Command();

    public interface IHandler
    {
        Task<List<OrderModel>?> Handle(Command cmd, CancellationToken ct);
    }

    public sealed class Handler(IOrderRepository repo) : IHandler
    {
        public async Task<List<OrderModel>?> Handle(Command cmd, CancellationToken ct)
        {   
            var product = await repo.ListAsync(ct);
            
            return product;
        }
    }
}