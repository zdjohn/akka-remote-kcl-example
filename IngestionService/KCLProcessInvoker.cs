using System.Collections.Generic;
using System.Diagnostics;
using Akka.Util.Internal;
using System.Configuration;
using System.IO;
using System.Linq;

namespace IngestionService
{
    public class KclProcessInvoker : IRemoteProcessInvoker
    {
        public bool Invoke()
        {
            //todo:1. should be set at app setting config instead here,
            //todo: you need to set up your own IAM and kinesis, and config your kcl.properties accordingly
            //how to invoke KCL see example: https://github.com/awslabs/amazon-kinesis-client-net/blob/master/Bootstrap/Bootstrap.cs

            var kiniesisJars = FetchJars(ConfigurationManager.AppSettings["jars_folder"]);
            var key = ConfigurationManager.AppSettings["access_key"];
            var secret = ConfigurationManager.AppSettings["secret"];
            var javaExcutable = ConfigurationManager.AppSettings["javaExcutable"];
            var processorName = ConfigurationManager.AppSettings["processorName"];
            var processInfo = new ProcessStartInfo(javaExcutable, $"-Daws.accessKeyId={key} -Daws.secretKey={secret} -cp {kiniesisJars} com.amazonaws.services.kinesis.multilang.MultiLangDaemon kcl.properties")
            {
                CreateNoWindow = false,
                UseShellExecute = false
            };
            var remoteProcesses = Process.GetProcessesByName(processorName);
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

        /// <summary>
        /// Downloads all the required jars from Maven and returns a classpath string that includes all those jars.
        /// </summary>
        /// <returns>Classpath string that includes all the jars downloaded.</returns>
        /// <param name="jarFolder">Folder into which to save the jars.</param>
        private static string FetchJars(string jarFolder)
        {
            if (jarFolder == null)
            {
                jarFolder = "jars";
            }

            if (!Path.IsPathRooted(jarFolder))
            {
                jarFolder = Path.Combine(Directory.GetCurrentDirectory(), jarFolder);
            }

            List<string> files = Directory.GetFiles(jarFolder).Where(f => f.EndsWith(".jar")).ToList();
            //files.Add(Directory.GetCurrentDirectory());
            files.Add(jarFolder);
            return string.Join(Path.PathSeparator.ToString(), files);
        }


    }
}