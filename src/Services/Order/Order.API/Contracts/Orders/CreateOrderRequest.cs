namespace Order.API.Contracts.Orders;

public sealed record CreateOrderRequest(List<CreateOrderItemRequest> Items, string ClientDocument);