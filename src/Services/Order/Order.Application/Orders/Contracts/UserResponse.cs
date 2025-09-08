namespace Order.Application.Orders.Contracts;

public sealed record UserResponse(Guid Id, string Name, string Email);