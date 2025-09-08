namespace Order.API.Contracts.Orders;

public sealed record OrderResponse(Guid Id, Guid UserId, string SellerName, string ClientDocument, List<OrderItemResponse> Items, decimal TotalAmount);