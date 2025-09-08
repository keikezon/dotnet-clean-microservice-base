using MassTransit;
using Microsoft.Extensions.Logging;
using Product.Application.Contracts.Events;
using Product.Application.Products.Abstractions;

namespace Product.Infrastructure.Messaging.Consumers;

public class OrderCreatedConsumer : IConsumer<StockUpdateIntegrationEvent>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(IProductRepository productRepository, ILogger<OrderCreatedConsumer> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StockUpdateIntegrationEvent> context)
    {
        try
        {
            var message = context.Message;

            _logger.LogInformation(
                "Recebido evento OrderCreated: ProductId={ProductId}, Quantity={Quantity}",
                message.ProductId,
                message.Quantity
            );

            // Atualiza o estoque do produto
            await _productRepository.DecreaseStockAsync(
                message.ProductId,
                message.Quantity,
                context.CancellationToken
            );

            _logger.LogInformation(
                "Estoque atualizado com sucesso para ProductId={ProductId}",
                message.ProductId
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar evento OrderCreated: {Message}", ex.Message);
            throw;
        }
    }
}