using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Autofac;

namespace IngestionService
{
    [ExcludeFromCodeCoverage]
    public class ServiceHost
    {
        
        public void Start(IContainer container)
        {
            IngestionActorSystem.Initialize(container);
        }

        public void Stop()
        {
            IngestionActorSystem.Shutdown();
        }
    }
}