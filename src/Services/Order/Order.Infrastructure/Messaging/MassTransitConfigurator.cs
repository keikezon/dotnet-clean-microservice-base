using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Order.Infrastructure.Messaging;

public static class MassTransitConfigurator
{
    public static void AddMassTransitWithRabbit(this IServiceCollection services, IConfiguration cfg)
    {
        var rabbitHost = cfg.GetConnectionString("RabbitMq") ?? "amqp://guest:guest@rabbitmq:5672";
        services.AddMassTransit(x =>
        {
            // Configura transporte RabbitMQ
            x.UsingRabbitMq((context, busCfg) =>
            {
                busCfg.Host(rabbitHost);

                // Cria a fila "order-created-queue" com dead-letter
                busCfg.ReceiveEndpoint("order-created-queuev2", e =>
                {
                    e.ConfigureConsumeTopology = false; // evita conflito de argumentos
                    e.Durable = true;
                    e.SetQueueArgument("x-dead-letter-exchange", "order-created-queue-dead");
                });
                busCfg.ReceiveEndpoint("order-created-queuev2-dead", e =>
                {
                    e.Durable = true; // exchange de dead-letter
                });
            });
        });
    }
}