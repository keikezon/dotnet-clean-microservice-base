using MassTransit;
using Product.Application.Contracts.Events;
using Product.Application.Products.Abstractions;
using Product.Domain.Products;

namespace Product.Application.Products.Commands;

public sealed class UpdateProduct
{
    public sealed record Command(Guid Id, string Name, string Description, decimal Price, int Stock);

    public interface IHandler
    {
        Task<ProductModel> Handle(Command cmd, CancellationToken ct);
    }

    public sealed class Handler(IProductRepository repo, IPublishEndpoint bus) : IHandler
    {
        public async Task<ProductModel> Handle(Command cmd, CancellationToken ct)
        {
            var product = ProductModel.Update(cmd.Id, cmd.Name, cmd.Description, cmd.Price, cmd.Stock);
            await repo.UpdateProductAsync(product, ct);

            var @event = new ProductUpdateIntegrationEvent(product.Id, product.Name);
            await bus.Publish(@event, ct);
            
            return product;
        }
    }
}