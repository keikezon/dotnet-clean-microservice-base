namespace Order.API.Contracts.Orders;

public sealed record OrderItemResponse(Guid Id, Guid ProductId, string Name, int Quantity, decimal UnitPrice);