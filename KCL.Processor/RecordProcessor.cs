﻿using System;
using System.Linq;
using Akka.Actor;
using Amazon.Kinesis.ClientLibrary;
using NLog;

namespace KCL.Processor
{
    public class RecordProcessor : IRecordProcessor
    {
        /// <value>The time to wait before this record processor
        /// reattempts either a checkpoint, or the processing of a record.</value>
        private static readonly TimeSpan Backoff = TimeSpan.FromSeconds(3);
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// This method is invoked by the Amazon Kinesis Client Library before records from the specified shard
        /// are delivered to this SampleRecordProcessor.
        /// </summary>
        /// <param name="input">
        /// InitializationInput containing information such as the name of the shard whose records this
        /// SampleRecordProcessor will process.
        /// </param>
        public void Initialize(InitializationInput input)
        {
            Logger.Debug("Initializing record processor for shard: " + input.ShardId);
        }

        public void ProcessRecords(ProcessRecordsInput input)
        {
            try
            {
                //push records to ingestion service receiver actor
                var isSuccess =
                     ConsumerActorSystem.RecordReceiver.Ask<bool>(input.Records, TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
                Logger.Info($"remote service health status: {isSuccess}");

                if (isSuccess)
                {
                    //check point on records successfullly sent accross.
                    input.Checkpointer.Checkpoint(RetryingCheckpointErrorHandler.Create(3, Backoff));
                }
                else
                {
                    //ingestion service unhealthy, shut down processor
                    Logger.Fatal("Nux: i live, i die, i live again");
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                //log exception
                Logger.Error(ex);
                Logger.Fatal("Nux: i live, i die, i live again");
                //shut down processor
                Environment.Exit(0);
            }
        }

        public void Shutdown(ShutdownInput input)
        {
            Logger.Debug("Shutting down record processor for shard");
            // Checkpoint after reaching end of shard, so we can start processing data from child shards.
            if (input.Reason == ShutdownReason.TERMINATE)
            {
                ConsumerActorSystem.Shutdown();
                input.Checkpointer.Checkpoint(errorHandler: RetryingCheckpointErrorHandler.Create(3, Backoff));
            }
        }
        
        
    }
    
}
