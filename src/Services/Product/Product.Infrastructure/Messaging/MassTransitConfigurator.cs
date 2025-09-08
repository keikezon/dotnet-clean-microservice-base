using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Infrastructure.Messaging.Consumers;

namespace Product.Infrastructure.Messaging;

public static class MassTransitConfigurator
{
    public static void AddMassTransitWithRabbit(this IServiceCollection services, IConfiguration cfg)
    {
        var rabbitHost = cfg.GetConnectionString("RabbitMq") ?? "amqp://guest:guest@rabbitmq:5672";

        services.AddMassTransit(x =>
        {
            // Adiciona o consumer
            x.AddConsumer<OrderCreatedConsumer>();

            x.UsingRabbitMq((context, busCfg) =>
            {
                busCfg.Host(rabbitHost);

                // Configura a fila para o consumer
                busCfg.ReceiveEndpoint("order-created-queue", e =>
                {
                    e.ConfigureConsumeTopology = false;
                    e.Durable = true;
                    e.SetQueueArgument("x-dead-letter-exchange", "order-created-queue-dead");
                });
                
                busCfg.ReceiveEndpoint("order-created-queue-dead", e =>
                {
                    e.Durable = true; // exchange de dead-letter
                });

                // busCfg.ConfigureEndpoints(context);
            });
        });

    }
}