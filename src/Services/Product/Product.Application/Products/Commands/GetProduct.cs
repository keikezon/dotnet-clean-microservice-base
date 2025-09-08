using Product.Application.Products.Abstractions;
using Product.Domain.Products;

namespace Product.Application.Products.Commands;

public sealed class GetProduct
{
    public sealed record Command(Guid Id);

    public interface IHandler
    {
        Task<ProductModel?> Handle(Command cmd, CancellationToken ct);
    }

    public sealed class Handler(IProductRepository repo) : IHandler
    {
        public async Task<ProductModel?> Handle(Command cmd, CancellationToken ct)
        {
            if (cmd.Id == Guid.Empty)
                throw new ArgumentException("Invalid Id.");
            
            var product = await repo.GetByIdAsync(cmd.Id, ct);
            
            return product;
        }
    }
}