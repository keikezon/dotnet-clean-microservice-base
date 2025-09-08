using Product.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Product.Application.Products.Abstractions;
using Product.Domain.Products;

namespace Product.Infrastructure.Repositories;

public sealed class ProductRepository(AppDbContext db) : IProductRepository
{
    public async Task AddAsync(ProductModel product, CancellationToken ct) => await db.Products.AddAsync(product, ct);

    public Task<ProductModel?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
    
    public async Task UpdateProductAsync(ProductModel product, CancellationToken ct)
    {
        var productEntity = await db.Products.FindAsync(product.Id);

        if (productEntity == null)
            throw new KeyNotFoundException("Produto não encontrado.");

        productEntity.Name = product.Name;
        productEntity.Description = product.Description;
        productEntity.Price = product.Price;
        productEntity.Stock = product.Stock;

        await db.SaveChangesAsync(ct);
    }

    
    public Task<List<ProductModel>> ListAsync(CancellationToken ct) =>
        db.Products.AsNoTracking().Where(w => w.Active == true).ToListAsync(cancellationToken: ct);
    
    public async Task DecreaseStockAsync(Guid id, int quantity, CancellationToken ct)
    {
        var productEntity = await db.Products.FindAsync(id);

        if (productEntity == null)
            throw new KeyNotFoundException("Product not found.");
        if(productEntity.Stock == 0)
            throw new KeyNotFoundException("Stock zero.");
        if((productEntity.Stock-quantity) < 0)
            throw new KeyNotFoundException("Stock not enought.");
        
        productEntity.Stock -= quantity;

        await db.SaveChangesAsync(ct);
    }
    
    public async Task IncreaseStockAsync(Guid id, int quantity, string invoice, CancellationToken ct)
    {
        var productEntity = await db.Products.FindAsync(id);

        if (productEntity == null)
            throw new KeyNotFoundException("Product not found.");
        
        productEntity.Stock += quantity;
        productEntity.Invoice = invoice;

        await db.SaveChangesAsync(ct);
    }
    
    public async Task<bool> DeleteProductAsync(Guid id, CancellationToken ct)
    {
        var productEntity = await db.Products.FindAsync(id);

        if (productEntity == null)
            throw new KeyNotFoundException("Product not found.");

        productEntity.Active = false;

        int affected = await db.SaveChangesAsync(ct);
        
        return affected > 0;
    }
}
