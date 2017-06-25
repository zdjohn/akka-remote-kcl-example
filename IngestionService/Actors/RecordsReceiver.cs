using System;
using System.Collections.Generic;
using Amazon.Kinesis.ClientLibrary;
using Akka.Actor;

namespace IngestionService
{
    public sealed class RecordsReceiver : BaseReceiverActor
    {
        private readonly IHealthyStatusService _healthyStatus;
        public RecordsReceiver(IHealthyStatusService healthyStatus) : base(healthyStatus)
        {
            _healthyStatus = healthyStatus;
            Ready();
        }

        protected override void Ready()
        {
            Log.Info($"{GetType()} is now ready");

            Receive<String>(msg =>
            {
                Log.Info(msg);
                _healthyStatus.LogKclTimestamp();
                Sender.Tell(_healthyStatus.IsHealthy);
            });

            Receive<List<Record>>(records =>
            {
                if (_healthyStatus.IsHealthy)
                {
                    //processing some logic here 
                    Log.Warning(
                        $"getting records.............................. {records.Count} in batch................... ");
                    //or pass to other actors
                    _healthyStatus.LogKclTimestamp();
                }
                else
                {
                    Log.Warning($"stashing in coming messages");
                    Stash.Stash();
                }
                Sender.Tell(_healthyStatus.IsHealthy);
            });
            base.Ready();
        }

        protected override void Pending()
        {
            base.Pending();
            ReceiveAny(x =>
            {
                Log.Warning($"stashing in coming messages");
                Stash.Stash();
                Sender.Tell(_healthyStatus.IsHealthy);
            });
        }

    }
}
