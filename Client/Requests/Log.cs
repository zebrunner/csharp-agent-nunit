namespace ZafiraIntegration.Client.Requests
{
    internal class Log
    {
        public long TestId { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public long Timestamp { get; set; }
    }
}