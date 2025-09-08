using System.Reflection.PortableExecutable;

namespace Product.Domain.Products;

public sealed class ProductModel
{
    public ProductModel() { }

    public ProductModel(Guid id, string name, string description, decimal price, int stock)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        Active = true;
    }

    public Guid Id { get; private set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? Invoice { get; set; }
    
    public bool Active { get; set; }

    public static ProductModel Create(string name, string description, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required", nameof(description));
        if (price <= 0) throw new ArgumentException("Price is required", nameof(price));
        if (stock < 0) throw new ArgumentException("Stock is required", nameof(stock));

        return new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description.Trim(),
            Price = price,
            Stock = stock,
            Active = true
        };
    }

    public static ProductModel Update(Guid id, string name, string description, decimal price, int stock)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id is required", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required", nameof(description));
        if (price <= 0) throw new ArgumentException("Price is required", nameof(price));
        if (stock < 0) throw new ArgumentException("Stock is required", nameof(stock));

        return new ProductModel
        {
            Id = id,
            Name = name.Trim(),
            Description = description.Trim(),
            Price = price,
            Stock = stock,
            Active = true
        };
    }
    
    public static ProductModel UpdateStock(Guid id, int stock, string? invoice)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id is required", nameof(id));
        if (stock < 0) throw new ArgumentException("Stock is required", nameof(stock));
        if (string.IsNullOrWhiteSpace(invoice)) throw new ArgumentException("Invoice is required", nameof(invoice));

        return new ProductModel
        {
            Id = id,
            Stock = stock,
            Invoice = invoice
        };
    }
    
    public static ProductModel DecreaseStock(Guid id, int stock)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id is required", nameof(id));
        if (stock < 0) throw new ArgumentException("Stock is required", nameof(stock));

        return new ProductModel
        {
            Id = id,
            Stock = stock
        };
    }
}
