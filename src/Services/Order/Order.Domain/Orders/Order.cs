using System.ComponentModel.DataAnnotations.Schema;

namespace Order.Domain.Orders;

public sealed class OrderModel
{
    private OrderModel() { }
    
    public OrderModel(Guid id, Guid userId, string? sellerName, string clientDocument, List<OrderItemModel> items,decimal totalAmount)
    {
        Id = id;
        UserId = userId;
        SellerName = sellerName;
        ClientDocument = clientDocument;
        Items = items;
        TotalAmount = totalAmount;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    [NotMapped]
    public string? SellerName { get; set; }
    public string ClientDocument { get; set; }
    public List<OrderItemModel> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }

    public static OrderModel Create(OrderModel orderModel)
    {
        if (string.IsNullOrWhiteSpace(orderModel.ClientDocument)) throw new ArgumentException("Client document is required", nameof(orderModel.ClientDocument));
        if (orderModel.Items.Count <= 0) throw new ArgumentException("Items is required", nameof(orderModel.Items));
        if (orderModel.UserId == Guid.Empty) throw new ArgumentException("UserId is required", nameof(orderModel.UserId));
        
        orderModel.CreatedAt = DateTime.UtcNow;
        orderModel.TotalAmount = orderModel.Items.Sum(x => x.UnitPrice * x.Quantity);
        return orderModel;
    }
}
