namespace Pulsus.Configuration
{
    public interface IEmailSettings
    {
        bool Enabled { get; set; }
        string To { get; set; }
        string From { get; set; }

		string Link { get; set; }

		string SmtpServer { get; set; }
		int SmtpPort { get; set; }
		string SmtpUsername { get; set; }
		string SmtpPassword { get; set; }
		bool SmtpEnableSsl { get; set; }
    }
}
