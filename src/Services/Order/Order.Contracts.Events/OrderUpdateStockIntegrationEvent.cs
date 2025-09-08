using Common.Messaging;

namespace Order.Contracts.Events;


public sealed class OrderUpdateStockIntegrationEvent
{
    public Guid ProductId { get; }
    public int Quantity { get; }
    
    public OrderUpdateStockIntegrationEvent() { }

    public OrderUpdateStockIntegrationEvent(Guid productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}