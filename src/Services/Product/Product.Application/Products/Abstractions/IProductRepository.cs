using Product.Domain.Products;

namespace Product.Application.Products.Abstractions;

public interface IProductRepository
{
    Task AddAsync(ProductModel product, CancellationToken ct);
    Task<ProductModel?> GetByIdAsync(Guid id, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);

    Task UpdateProductAsync(ProductModel product, CancellationToken ct);
    Task<List<ProductModel>> ListAsync(CancellationToken ct);
    
    Task DecreaseStockAsync(Guid id, int quantity, CancellationToken ct);
    Task IncreaseStockAsync(Guid id, int quantity, string invoice, CancellationToken ct);

    Task<bool> DeleteProductAsync(Guid id, CancellationToken ct);
}
