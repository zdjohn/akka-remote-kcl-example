using Amazon.Kinesis.ClientLibrary;
using Autofac;

namespace KCL.Processor.Autofac
{
    public class Dependencies : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RecordProcessor>().As<IRecordProcessor>().SingleInstance();
            
        }
    }
}
