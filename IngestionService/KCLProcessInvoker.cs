using System.Diagnostics;
using Akka.Util.Internal;

namespace IngestionService
{
    public class KclProcessInvoker : IRemoteProcessInvoker
    {
        public bool Invoke()
        {
            //todo: should be set at app setting config instead here, 
            //and change the folder path to relative of  excutable

            //see example: https://github.com/awslabs/amazon-kinesis-client-net/blob/master/Bootstrap/Bootstrap.cs
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