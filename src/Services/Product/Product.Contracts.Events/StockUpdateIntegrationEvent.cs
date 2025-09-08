using Common.Abstractions;
using Common.Messaging;

namespace Product.Application.Contracts.Events;

public class StockUpdateIntegrationEvent
{
    public Guid ProductId { get; }
    public int Quantity { get; }

    public StockUpdateIntegrationEvent() { }

    public StockUpdateIntegrationEvent(Guid productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}