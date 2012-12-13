namespace Pulsus.Configuration
{
    public class ServerSettings : IServerSettings
    {
        public ServerSettings()
        {
        }

        internal ServerSettings(ServerElement remoteElement)
        {
            Enabled = remoteElement.Enabled;
			ApiKey = remoteElement.ApiKey;
            Url = remoteElement.Url;
            Compress = remoteElement.Compress;
        }

        public bool Enabled { get; set; }
		public string ApiKey { get; set; }
        public string Url { get; set; }
        public bool Compress { get; set; }
    }
}
