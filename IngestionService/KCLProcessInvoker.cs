using System.Diagnostics;
using Akka.Util.Internal;

namespace IngestionService
{
    public class KclProcessInvoker : IRemoteProcessInvoker
    {
        public bool Invoke()
        {
            //todo:1. should be set at app setting config instead here,
            //todo: you need to set up your own IAM and kinesis, and config your kcl.properties accordingly
            //how to invoke KCL see example: https://github.com/awslabs/amazon-kinesis-client-net/blob/master/Bootstrap/Bootstrap.cs

            var kiniesisJars = "amazon-kinesis-client-1.6.5.jar...";
            var key = "";
            var secret = "";
            var kcl_properties = "kcl.properties";
            var javaExcutable = "C:\\Program Files\\Java\\jdk1.8.0_112\\bin\\java.exe";
            var processName = "KCL.Processor";
            var processInfo = new ProcessStartInfo(javaExcutable, $"-Daws.accessKeyId={key} -Daws.secretKey={secret} -cp {kiniesisJars} {kcl_properties}")
            {
                CreateNoWindow = false,
                UseShellExecute = false
            };
            var remoteProcesses = Process.GetProcessesByName(processName);
            if (remoteProcesses.Length > 0)
            {
                remoteProcesses.ForEach(x => x.Kill());
            }

            var process = Process.Start(processInfo);
            if (process?.Id > 0)
            {
                return true;
            }
            return false;
        }
    }
}