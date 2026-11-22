using MassTransit;

namespace ProgressService.Messaging
{
    public static class MessagingExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.RegisterConsumers();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(
                        configuration["RabbitMq:Host"] ?? "localhost",
                        configuration["RabbitMq:VHost"] ?? "Elevate", h =>
                        {
                            h.Username(configuration["RabbitMq:Username"] ?? "guest");
                            h.Password(configuration["RabbitMq:Password"] ?? "guest");
                        });

                    // Configure the receive endpoints for the consumers :
                    cfg.ConfigureReceiveEndpoints(context);
                });
            });
            return services;
        }
    }
}
