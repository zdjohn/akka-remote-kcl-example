using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Amazon.Kinesis.ClientLibrary;
using Autofac;

namespace KCL.Processor
{
    class Program
    {
        /// <summary>
        /// This method creates a KclProcess and starts running a Processor instance.
        /// </summary>
        public static void Main(string[] args)
        {
            //set up actor system
            ConsumerActorSystem.Initialize();

            var container = GetContainer();
            var processor = container.Resolve<IRecordProcessor>();
            try
            {
                KclProcess.Create(processor).Run();
            }
            catch (Exception e)
            {
                Console.WriteLine($"i live, i die, i live again {e.Message} {e.InnerException?.Message}");
                Environment.Exit(-1);
            }
        }

        private static IContainer GetContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            return builder.Build();
        }
    }
}
