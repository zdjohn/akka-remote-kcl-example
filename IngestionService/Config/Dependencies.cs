using Autofac;

namespace IngestionService.Config
{
    public class Dependencies : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthyStatusService>().As<IHealthyStatusService>().SingleInstance();
            builder.RegisterType<RemoteProcessInvoker>().As<IRemoteProcessInvoker>().SingleInstance();
            builder.RegisterType<ServiceHost>().InstancePerLifetimeScope();
            builder.RegisterType<HealthCheckerActor>().SingleInstance();
            builder.RegisterType<RecordsReceiver>();
        }
    }
}
