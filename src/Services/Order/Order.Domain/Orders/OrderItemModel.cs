using System.ComponentModel.DataAnnotations.Schema;

namespace Order.Domain.Orders;

public class OrderItemModel
{
    private OrderItemModel() { }

    public OrderItemModel(Guid id, Guid orderId, Guid productId, int quantity, decimal unitPrice)
    {
        Id = id;
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    [NotMapped]
    public string? Name { get; set; }

    public static OrderItemModel Create(Guid orderId, Guid productId, int quantity, decimal unitPrice)
    {
        if (orderId == Guid.Empty) throw new ArgumentException("Order id is required", nameof(quantity));
        if (productId == Guid.Empty) throw new ArgumentException("Product id is required", nameof(quantity));
        if (quantity <= 0) throw new ArgumentException("Quantity is required", nameof(quantity));
        if (unitPrice < 0) throw new ArgumentException("Price is required", nameof(unitPrice));

        return new OrderItemModel
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }

    public static OrderItemModel Update(Guid id, Guid orderId, Guid productId, int quantity, decimal unitPrice)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id is required", nameof(id));
        if (orderId == Guid.Empty) throw new ArgumentException("Order id is required", nameof(quantity));
        if (productId == Guid.Empty) throw new ArgumentException("Product id is required", nameof(quantity));
        if (quantity <= 0) throw new ArgumentException("Quantity is required", nameof(quantity));
        if (unitPrice < 0) throw new ArgumentException("Unit price is required", nameof(unitPrice));

        return new OrderItemModel
        {
            Id = id,
            OrderId = orderId,
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }
}