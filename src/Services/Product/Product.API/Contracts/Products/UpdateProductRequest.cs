using Common.Enum;

namespace Identity.API.Contracts.Products;

public sealed record UpdateProductRequest(Guid Id, string Name, string Description, decimal Price, int Stock);
