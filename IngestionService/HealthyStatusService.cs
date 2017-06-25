using System;
using System.IO;

namespace IngestionService
{
    /// <summary>
    /// used as a singleton, under this service, 
    /// we can implement all the business logic of:
    /// how and what is a "HEALTHY" state
    /// </summary>
    public interface IHealthyStatusService
    {
        bool CheckHealthy();
        bool IsHealthy { get; }
        bool IsRemoteIdle { get; }
        DateTime KclTimeStamp { get; }
        void LogKclTimestamp();
    }

    public class HealthyStatusService : IHealthyStatusService
    {
        private static readonly TimeSpan KclMaxIdleTime= TimeSpan.FromSeconds(100);
        private DateTime _kclTimestamp = default(DateTime);

        public bool CheckHealthy()
        {
            //impeletemt your health status logic here
            IsHealthy = checkApiStatus() && checkDbStatus();
            return IsHealthy;
        }

        public bool IsHealthy { get; private set; }

        public DateTime KclTimeStamp => _kclTimestamp;

        public bool IsRemoteIdle
            => false;
        //(DateTime.UtcNow - _kclTimestamp) > KclMaxIdleTime;
        
        public void LogKclTimestamp()
        {
            _kclTimestamp = DateTime.UtcNow;
        }

        private bool checkApiStatus()
        {
            //api health check logic here
            var random = new Random();
            var x = random.Next();
            //random faliure: set to 10% of chance api status fail
            //return x%10 != 0;
            return true;
        }

        private bool checkDbStatus()
        {
            //db store check logic here
            return File.Exists("healthy.txt");
        }
    }

    
}
