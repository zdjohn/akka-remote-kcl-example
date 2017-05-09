using System;
using Akka.Actor;
using Amazon.Kinesis.ClientLibrary;

namespace KCL.Processor
{
    public class RecordProcessor : IRecordProcessor
    {
        /// <value>The time to wait before this record processor
        /// reattempts either a checkpoint, or the processing of a record.</value>
        private static readonly TimeSpan Backoff = TimeSpan.FromSeconds(3);
        
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
            Console.Error.WriteLine("Initializing record processor for shard: " + input.ShardId);
        }

        public async void ProcessRecords(ProcessRecordsInput input)
        {
            try
            {
                //push records to ingestion service receiver actor
                var isSuccess =
                    await ConsumerActorSystem.RecordReceiver.Ask<bool>(input.Records, TimeSpan.FromSeconds(5));

                //ingestion service unhealthy, shut down processor
                if (!isSuccess)
                {
                    Console.WriteLine("i live, i die, i live again");
                    Environment.Exit(0);
                }

                input.Checkpointer.Checkpoint(errorHandler: RetryingCheckpointErrorHandler.Create(3, Backoff));
            }
            catch (Exception ex)
            {
                //log exception
                Console.WriteLine(ex.Message);
                //shut down processor
                Environment.Exit(0);
            }
        }

        public void Shutdown(ShutdownInput input)
        {
            Console.Error.WriteLine("Shutting down record processor for shard");
            // Checkpoint after reaching end of shard, so we can start processing data from child shards.
            if (input.Reason == ShutdownReason.TERMINATE)
            {
                ConsumerActorSystem.Shutdown();
                input.Checkpointer.Checkpoint(errorHandler: RetryingCheckpointErrorHandler.Create(3, Backoff));
            }
        }
        
        
    }
    
}
