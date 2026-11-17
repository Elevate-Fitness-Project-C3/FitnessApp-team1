using MassTransit;
using ProgressService.Features.completedworkoutRecordFeature.Consumers;

namespace ProgressService.Messaging
{
    public static class ConsumersRegistration
    {
        public static void RegisterConsumers(this IBusRegistrationConfigurator x)
        {
            x.AddConsumer<WorkoutCompletedConsumer>();
        }
    }
}
