using Product.Application.Products.Abstractions;
using Product.Domain.Products;

namespace Product.Application.Products.Commands;

public sealed class DeleteProduct
{
    public sealed record Command(Guid Id);

    public interface IHandler
    {
        Task<bool> Handle(Command cmd, CancellationToken ct);
    }

    public sealed class Handler(IProductRepository repo) : IHandler
    {
        public async Task<bool> Handle(Command cmd, CancellationToken ct)
        {
            if (cmd.Id == Guid.Empty)
                throw new ArgumentException("Invalid Id.");
            
            var deleted = await repo.DeleteProductAsync(cmd.Id, ct);
            
            return deleted;
        }
    }
}