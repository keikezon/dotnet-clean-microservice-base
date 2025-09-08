using AutoMapper;
using Order.API.Contracts.Orders;
using Order.Domain.Orders;

namespace Order.API.Mappings;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<OrderItemModel, OrderItemResponse>();
        CreateMap<OrderModel, OrderResponse>();
        CreateMap<CreateOrderItemRequest, OrderItemModel>();
        CreateMap<CreateOrderRequest, OrderModel>();
    }
}