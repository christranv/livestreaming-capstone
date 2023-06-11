namespace Stream.API
{
    public class StreamSettings
    {
        public string ConnectionString { get; set; }
        public string EventBusConnection { get; set; }
        public string SrsRtmpServerUrl { get; set; }
        public string SrsApiServerUrl { get; set; }
    }
}
