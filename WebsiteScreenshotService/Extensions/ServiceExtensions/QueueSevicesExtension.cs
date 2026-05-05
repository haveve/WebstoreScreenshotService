using MassTransit;
using Microsoft.Extensions.Options;
using WebsiteScreenshotService.Configurations;

namespace WebsiteScreenshotService.Extensions.ServiceExtensions;

public static class QueueSevicesExtension
{
    public static IServiceCollection AddMessageBrokerMassTransit(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumersFromNamespaceContaining<object>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var config = context.GetRequiredService<IOptions<MessageBrokerConfigurations>>().Value;
                ConfigureRabbitMq(cfg, context, config);
            });
        });

        return services;
    }

    private static void ConfigureRabbitMq(
        IRabbitMqBusFactoryConfigurator cfg,
        IBusRegistrationContext context,
        MessageBrokerConfigurations config)
    {
        cfg.Host(config.Connection.HostName, config.Connection.Port, config.Connection.VirtualHost, h =>
        {
            h.Username(config.Connection.UserName);
            h.Password(config.Connection.Password);
        });

        // 🚀 performance tuning defaults
        cfg.PrefetchCount = 16;
        cfg.ConcurrentMessageLimit = 32;

        // retry policy (production-safe default)
        cfg.UseMessageRetry(r =>
        {
            r.Exponential(
                retryLimit: 5,
                minInterval: TimeSpan.FromMilliseconds(200),
                maxInterval: TimeSpan.FromSeconds(10),
                intervalDelta: TimeSpan.FromSeconds(1));
        });

        // optional delayed redelivery
        cfg.UseDelayedRedelivery(r =>
        {
            r.Intervals(
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(30),
                TimeSpan.FromMinutes(1));
        });

        // automatically configure endpoints for registered consumers
        cfg.ConfigureEndpoints(context);
    }

}
