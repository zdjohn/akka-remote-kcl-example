using Akka.Actor;

namespace KCL.Processor
{
    public static class ConsumerActorSystem
    {
        private static ActorSystem _system;
        public static ActorSelection RecordReceiver;

        public static string StatsServices = "akka.tcp://ingestion-ws@localhost:8080";
        public static void Initialize()
        {
            _system = ActorSystem.Create("kcl-processor");
            RecordReceiver = _system.ActorSelection($"{StatsServices}/user/record-receiver");
        }

        public static void Shutdown()
        {
            if (_system != null)
            {
                _system.Terminate();
                _system = null;
            }
        }
    }
}
