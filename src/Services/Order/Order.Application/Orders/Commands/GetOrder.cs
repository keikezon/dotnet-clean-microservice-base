using Order.Application.Orders.Abstractions;
using Order.Domain.Orders;

namespace Order.Application.Orders.Commands;

public sealed class GetOrder
{
    public sealed record Command(Guid Id);

    public interface IHandler
    {
        Task<OrderModel?> Handle(Command cmd, CancellationToken ct);
    }

    public sealed class Handler(IOrderRepository repo) : IHandler
    {
        public async Task<OrderModel?> Handle(Command cmd, CancellationToken ct)
        {
            if (cmd.Id == Guid.Empty)
                throw new ArgumentException("Invalid Id.");
            
            var order = await repo.GetByIdAsync(cmd.Id, ct);
            
            return order;
        }
    }
}