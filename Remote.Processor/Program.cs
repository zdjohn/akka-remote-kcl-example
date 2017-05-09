using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;

namespace Remote.Processor
{
    class Program
    {
        public const string StatsServices = "akka.tcp://ingestion-ws@localhost:8080";
        static void Main(string[] args)
        {
            
            using (var system = ActorSystem.Create("remote-processor"))
            {
                var recordReceiver = system.ActorSelection($"{StatsServices}/user/record-receiver");
                Task.Run(async () =>
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Thread.Sleep(300);
                        try
                        {
                            var isRemoteHealthy =
                                await recordReceiver.Ask<bool>($"message from processor: {i}", TimeSpan.FromSeconds(5));
                            if (!isRemoteHealthy)
                            {
                                Environment.Exit(0);
                            }
                            recordReceiver.Tell($"{i} processed");
                        }
                        catch (Exception ex)
                        {
                            //if remote service time out
                            Environment.Exit(0);
                        }

                    }
                }).GetAwaiter().GetResult();
            }

            //Console.ReadLine();

        }
    }
}
