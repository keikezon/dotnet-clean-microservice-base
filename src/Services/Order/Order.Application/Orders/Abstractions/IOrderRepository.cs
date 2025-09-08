using Order.Domain.Orders;

namespace Order.Application.Orders.Abstractions;

public interface IOrderRepository
{
    Task AddAsync(OrderModel order, CancellationToken ct);
    Task<OrderModel> GetByIdAsync(Guid id, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<List<OrderModel>> ListAsync(CancellationToken ct);
}
