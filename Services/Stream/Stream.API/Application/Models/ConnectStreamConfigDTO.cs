namespace Stream.API.Application.Models
{
    public class ConnectStreamConfigDTO
    {
        public string ServerUrl { get; }
        public string StreamKey { get; }

        public ConnectStreamConfigDTO(string serverUrl, string streamKey)
        {
            ServerUrl = serverUrl;
            StreamKey = streamKey;
        }
    }
}