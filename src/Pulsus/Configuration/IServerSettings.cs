namespace Pulsus.Configuration
{
    public interface IServerSettings
    {
        bool Enabled { get; set; }
		string ApiKey { get; set; }
        string Url { get; set; }
        bool Compress { get; set; }
    }
}