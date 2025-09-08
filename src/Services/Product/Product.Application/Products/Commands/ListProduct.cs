using Product.Application.Products.Abstractions;
using Product.Domain.Products;

namespace Product.Application.Products.Commands;

public sealed class ListProduct
{
    public sealed record Command();

    public interface IHandler
    {
        Task<List<ProductModel>?> Handle(Command cmd, CancellationToken ct);
    }

    public sealed class Handler(IProductRepository repo) : IHandler
    {
        public async Task<List<ProductModel>?> Handle(Command cmd, CancellationToken ct)
        {   
            var product = await repo.ListAsync(ct);
            
            return product;
        }
    }
}