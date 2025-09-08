using MassTransit;
using Product.Application.Contracts.Events;
using Product.Application.Products.Abstractions;
using Product.Domain.Products;

namespace Product.Application.Products.Commands;

public sealed class UpdateStock
{
    public sealed record Command(Guid Id, int Quantity, string Invoice);

    public interface IHandler
    {
        Task<ProductModel> Handle(Command cmd, CancellationToken ct);
    }

    public sealed class Handler(IProductRepository repo, IPublishEndpoint bus) : IHandler
    {
        public async Task<ProductModel> Handle(Command cmd, CancellationToken ct)
        {
            var product = ProductModel.UpdateStock(cmd.Id, cmd.Quantity, cmd.Invoice);
            await repo.IncreaseStockAsync(product.Id, product.Stock, product.Invoice!, ct);

            // Publica evento via contrato
            var @event = new StockUpdateIntegrationEvent(product.Id, product.Stock);
            await bus.Publish(@event, ct);
            
            return product;
        }
    }
}