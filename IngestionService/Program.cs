using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Topshelf;

namespace IngestionService
{
    class Program
    {
        static int Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                var container = GetContainer();
                //todo: set up topshelf for events processor

                x.Service<ServiceHost>(s =>
                {
                    s.ConstructUsing(() =>
                    {
                        // http://haacked.com/archive/2004/06/28/current-directory-for-windows-service-is-not-what-you-expect.aspx
                        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                        return container.Resolve<ServiceHost>();
                    });
                    s.WhenStarted(tc => tc.Start(container));
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsNetworkService();
                x.EnablePauseAndContinue();
                x.SetDescription("Ingestion Service Sample");
                x.SetDisplayName("Ingestion Service Sample");
                x.SetServiceName(Assembly.GetExecutingAssembly().GetName().Name);
                x.EnableServiceRecovery(r => r.RestartService(1));
            });

            return (int)exitCode;
        }

        private static IContainer GetContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            return builder.Build();
        }
    }
}
