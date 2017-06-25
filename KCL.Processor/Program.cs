using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Amazon.Kinesis.ClientLibrary;
using Autofac;
using NLog;

namespace KCL.Processor
{
    class Program
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// This method creates a KclProcess and starts running a Processor instance.
        /// </summary>
        public static void Main(string[] args)
        {

            //set up actor system
            ConsumerActorSystem.Initialize();
            try
            {
                KclProcess.Create(new RecordProcessor()).Run();
            }
            catch (Exception e)
            {
                Logger.Error($"{e.Message} {e.InnerException?.Message}");
                Environment.Exit(-1);
            }
        }
        
    }
}
