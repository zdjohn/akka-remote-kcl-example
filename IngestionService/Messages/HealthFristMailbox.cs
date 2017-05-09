using Akka.Actor;
using Akka.Dispatch;

namespace IngestionService.Messages
{
    public class HealthFristMailbox : UnboundedPriorityMailbox
    {
        public HealthFristMailbox(Settings settings, Akka.Configuration.Config config) : base(settings, config)
        {
        }

        protected override int PriorityGenerator(object message)
        {
            var healthyStatus = message as HealthyStatus;
            return healthyStatus != null ? 0 : 1;
        }

        
    }
}
