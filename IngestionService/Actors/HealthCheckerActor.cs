using System.Threading;
using Akka.Actor;
using Akka.Event;
using IngestionService.Messages;

namespace IngestionService
{
    public class HealthCheckerActor : ReceiveActor
    {
        private readonly IHealthyStatusService _healthyStatus;
        private readonly IRemoteProcessInvoker _invoker;

        
        protected readonly ILoggingAdapter Log = Context.GetLogger();
        public HealthCheckerActor(IHealthyStatusService servicesHealthyStatus, IRemoteProcessInvoker invoker)
        {
            _healthyStatus = servicesHealthyStatus;
            _invoker = invoker;
            Yellow();
        }

        private void Red()
        {
            Log.Debug($"i am turned Red :-(");
            //publish healthy Status false
            Receive<CheckStatus>(x =>
            {
                var isHealthy = _healthyStatus.CheckHealthy();
                Log.Info($"i am in Red, status: {isHealthy}");
                Context.System.EventStream.Publish(new HealthyStatus(isHealthy));
                if (isHealthy)
                    Become(Yellow);
            });
            ReceiveAny(x =>
            {
                Log.Info($"{x.GetType()} not working??");
            });
        }

        private void Green()
        {
            Log.Debug($"i turned Green now");

            Receive<CheckStatus>(x =>
            {
                
                var isHealthy = _healthyStatus.CheckHealthy();
                Log.Info($"i am in Green, status: {isHealthy}");
                Context.System.EventStream.Publish(new HealthyStatus(isHealthy));
                if (!isHealthy)
                {
                    Become(Red);
                }else if (_healthyStatus.IsRemoteIdle)
                {
                    Become(Yellow);
                }
            });
            
        }

        private void Yellow()
        {
            Log.Debug($"i turned in Yellow");
            if (_healthyStatus.IsHealthy)
            {
                Log.Info("try to start remote processor...");

                if (_invoker.Invoke())
                {
                    Log.Info("involked success...");
                    //_healthyStatus.LogKclTimestamp();
                    Become(Green);
                }
            }
            else
            {
                Become(Red);
            }
            Receive<CheckStatus>(x =>
            {
                var isHealthy = _healthyStatus.CheckHealthy();
                if (isHealthy)
                {
                    Become(Green);
                }
                else
                {
                    Become(Red);
                }
            });
        }

    }
}
