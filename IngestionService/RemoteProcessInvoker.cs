using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Util.Internal;
using NLog.Fluent;

namespace IngestionService
{
    public class RemoteProcessInvoker: IRemoteProcessInvoker
    {
        public bool Invoke()
        {
            var path = 
                Path.GetFullPath(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, 
                    @"..\..\..\Remote.Processor\bin\Debug\Remote.Processor.exe"));
            var processInfo = new ProcessStartInfo(path)
            {
                CreateNoWindow = false,
                UseShellExecute = false,
            };
            var remoteProcesses = Process.GetProcessesByName("Remote.Processor");
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
