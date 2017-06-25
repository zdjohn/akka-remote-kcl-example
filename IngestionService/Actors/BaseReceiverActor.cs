using System;
using Akka.Actor;
using Akka.Event;
using IngestionService.Messages;

namespace IngestionService
{
    

    public abstract class BaseReceiverActor : ReceiveActor, IWithUnboundedStash
    {
        protected readonly ILoggingAdapter Log = Context.GetLogger();
        private readonly IHealthyStatusService _healthyStatus;

        protected BaseReceiverActor(IHealthyStatusService healthyStatus)
        {
            _healthyStatus = healthyStatus;
        }

        public IStash Stash { get; set; }
        
        protected virtual void Ready()
        {
            Log.Info("Actor status: Ready");

            Receive<HealthyStatus>(x =>
            {
                if (!_healthyStatus.IsHealthy)
                {
                    Log.Warning($"unhealthy message received, turning into pending mode {DateTime.UtcNow}");
                    Become(Pending);
                }
            });
            Log.Info($"un stashing messages");
            Stash?.UnstashAll();
        }

        protected virtual void Pending()
        {
            Log.Warning("Actor status: Pending");

            Receive<HealthyStatus>(x =>
            {
                if (_healthyStatus.IsHealthy)
                {
                    Log.Warning($"healthy message received, turning into healthy mode  {DateTime.UtcNow}");
                    Become(Ready);
                }
            });
            
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy( //or AllForOneStrategy
                maxNrOfRetries: 10,
                withinTimeRange: TimeSpan.FromSeconds(30),
                decider: Decider.From(x => Directive.Restart));
        }

        protected override void PreStart()
        {
            Context.System.EventStream.Subscribe(Self, typeof(HealthyStatus));
            base.PreStart();
        }
        
        protected override void PreRestart(Exception reason, object message)
        {
            Log?.Error(reason, $"{GetType()} exception", message);
            Context.System.EventStream.Unsubscribe(Self, typeof(HealthyStatus));
            Self.Tell(message);
            base.PreRestart(reason, message);
        }
    }
}
