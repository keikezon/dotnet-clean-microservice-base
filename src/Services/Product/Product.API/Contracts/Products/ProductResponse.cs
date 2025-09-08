namespace Product.API.Contracts.Products;

public sealed record ProductResponse(Guid Id, string Name, string Description, decimal Price, int Stock);