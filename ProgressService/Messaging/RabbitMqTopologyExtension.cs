using MassTransit;
using ProgressService.Features.completedworkoutRecordFeature.Consumers;

namespace ProgressService.Messaging
{
    public static class RabbitMqTopologyExtension
    {
        public static void ConfigureReceiveEndpoints(this IRabbitMqBusFactoryConfigurator cfg , IBusRegistrationContext context)
        {
            #region Workout Completed (Training -> Progress) :
            cfg.ReceiveEndpoint("progress-workout-completed", e =>
            {
                e.UseMessageRetry(r =>
                {
                    r.Ignore<ArgumentException>(); 
                    r.Interval(3, TimeSpan.FromSeconds(1));
                });
                e.ConfigureConsumer<WorkoutCompletedConsumer>(context);
            });

            #endregion
        }
    }
}
