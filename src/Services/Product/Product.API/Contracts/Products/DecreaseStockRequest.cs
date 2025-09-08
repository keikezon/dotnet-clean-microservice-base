namespace Product.API.Contracts.Products;

public record DecreaseStockRequest(Guid Id, int Quantity);