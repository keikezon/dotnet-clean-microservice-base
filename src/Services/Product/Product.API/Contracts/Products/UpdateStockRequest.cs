namespace Product.API.Contracts.Products;

public record UpdateStockRequest(Guid Id, int Quantity, string Invoice);