namespace IngestionService.Messages
{
    public class HealthyStatus
    {
        public HealthyStatus(bool isOk)
        {
            IsOk = isOk;
        }

        public bool IsOk { get; }
    }
}