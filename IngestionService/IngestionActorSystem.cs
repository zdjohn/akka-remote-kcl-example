using System;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using IngestionService.Messages;

namespace IngestionService
{
    public static class IngestionActorSystem
    {
        private static ActorSystem _system;
        public static IActorRef RecordsReceiver = ActorRefs.Nobody;
        public static IActorRef HealthChecker = ActorRefs.Nobody;
        public static ActorSystem System
        {
            get
            {
                if (_system == null) throw new InvalidOperationException("Actor system is not initialized");
                return _system;
            }
        }

        public static void Initialize(IContainer container)
        {
            _system = ActorSystem.Create("ingestion-ws");

            var propsResolver = new AutoFacDependencyResolver(container, _system);

            var recordsProcessorProp = _system.DI().Props<RecordsReceiver>();
            RecordsReceiver = _system.ActorOf(recordsProcessorProp, "record-receiver");

            var healthCheckerProps = _system.DI().Props<HealthCheckerActor>();
            HealthChecker = _system.ActorOf(healthCheckerProps, "health-checker");

            //actors system initialized
            _system.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(3),
                HealthChecker, new CheckStatus(), ActorRefs.NoSender);
        }

        public static void Shutdown()
        {
            _system?.Terminate().ContinueWith(task =>
            {
                _system = null;
            }).Wait();
        }
    }
}
