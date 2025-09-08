using MassTransit;
using Product.Application.Contracts.Events;
using Product.Application.Products.Abstractions;
using Product.Domain.Products;

namespace Product.Application.Products.Commands;

public sealed class DecreaseStock
{
    public sealed record Command(Guid Id, int Quantity);

    public interface IHandler
    {
        Task<ProductModel> Handle(Command cmd, CancellationToken ct);
    }

    public sealed class Handler(IProductRepository repo, IPublishEndpoint bus) : IHandler
    {
        public async Task<ProductModel> Handle(Command cmd, CancellationToken ct)
        {
            var product = ProductModel.DecreaseStock(cmd.Id, cmd.Quantity);
            await repo.DecreaseStockAsync(product.Id, product.Stock,ct);

            // Publica evento via contrato
            var @event = new StockUpdateIntegrationEvent(product.Id, product.Stock);
            await bus.Publish(@event, ct);
            
            return product;
        }
    }
}