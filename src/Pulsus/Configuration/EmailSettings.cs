namespace Pulsus.Configuration
{
    public class EmailSettings : IEmailSettings
    {
        public EmailSettings()
        {
        }

        internal EmailSettings(EmailElement emailElement)
        {
            Enabled = emailElement.Enabled;
            To = emailElement.To;
            From = emailElement.From;
			Link = emailElement.Link;

			SmtpServer = emailElement.SmtpServer;
			SmtpPort = emailElement.SmtpPort;
			SmtpUsername = emailElement.SmtpUsername;
			SmtpPassword = emailElement.SmtpPassword;
			SmtpEnableSsl = emailElement.SmtpEnableSsl;
        }

        public bool Enabled { get; set; }
        public string To { get; set; }
        public string From { get; set; }

		public string Link { get; set; }

		public string SmtpServer { get; set; }
		public int SmtpPort { get; set; }
		public string SmtpUsername { get; set; }
		public string SmtpPassword { get; set; }
		public bool SmtpEnableSsl { get; set; }
    }
}
