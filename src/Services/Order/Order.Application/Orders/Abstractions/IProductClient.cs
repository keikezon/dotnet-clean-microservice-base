using Order.Application.Orders.Contracts;

namespace Order.Application.Orders.Abstractions;

public interface IProductClient
{
    Task<bool> DecreaseStockAsync(Guid productId, int quantity, CancellationToken ct);
    Task<ProductResponse?> GetByIdAsync(Guid productId, CancellationToken ct);
}