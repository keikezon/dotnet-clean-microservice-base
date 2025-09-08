using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Messaging;

public static class MassTransitConfigurator
{
    public static void AddMassTransitWithRabbit(this IServiceCollection services, IConfiguration cfg)
    {
        var rabbitHost = cfg.GetConnectionString("RabbitMq") ?? "amqp://guest:guest@rabbitmq:5672";
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, busCfg) =>
            {
                busCfg.Host(rabbitHost);
                busCfg.ConfigureEndpoints(context);
            });
        });
    }
}
