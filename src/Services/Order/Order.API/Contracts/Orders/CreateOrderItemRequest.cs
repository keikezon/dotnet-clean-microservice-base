namespace Order.API.Contracts.Orders;

public sealed record CreateOrderItemRequest(Guid ProductId, int Quantity);